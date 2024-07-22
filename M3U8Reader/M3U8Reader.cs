using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace M3U8Reader;

/// <summary>
///     Supports you in reading M3U8 files. Note that this reader is not particularly hardened for malformed M3U8 files.
/// </summary>
/// <param name="stream">the stream that contains the file. Note that the reader leaves the stream open - you need to dispose of it yourself</param>
public class M3U8Reader(Stream stream) : IEnumerable<M3U8Line>
{
	public IEnumerator<M3U8Line> GetEnumerator()
	{
		var sr = new StreamReader(stream);
		while (!sr.EndOfStream)
		{
			
			yield return new M3U8Line(sr.ReadLine()!);
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
///		Represents a line in a M3U8 file. The line can be empty, a source line or an extension line.
/// </summary>
public readonly struct M3U8Line(string line)
{
	public static readonly M3U8Line Empty = new(string.Empty);

	public M3U8LineKind LineKind { get; } = line switch
	{
		{Length: 0} => M3U8LineKind.Empty,
		not null when line[0] == '#' => M3U8LineKind.Extension,
		_ => M3U8LineKind.Source
	};

	// ReSharper disable once MemberCanBePrivate.Global
	public string Value { get; } = line!;

	/// <summary>
	///		If the line is an extension, this property gives you access to the extension name and its values.
	///		Don't use this property if the line is not an extension (check <see cref="LineKind"/>).
	/// </summary>
	/// <exception cref="InvalidDataException">thrown if attempting to read a source line as extension line</exception>
	public M3U8Extension Extension => LineKind == M3U8LineKind.Extension ? new M3U8Extension(Value.AsSpan(1)) : throw new InvalidDataException();
}

public readonly ref struct M3U8Extension(ReadOnlySpan<char> line)
{
	private readonly ReadOnlySpan<char> input = line;

	/// <summary>
	///		Access to the 'name' of the extension. This can be basically the whole line (no further info)
	///		of the part before the colon, when additional information is associated with this extension.
	/// </summary>
	public string ExtensionName => (input[^1] == ':'
		? input[..^1]
		: input.IndexOf(':') switch
		{
			-1 => input,
			var index => input[..index]
		}).ToString();
	
	public ExtensionValues Values => new(input.Slice(ExtensionName.Length + 1));
}

/// <summary>
///		If the extension has values, you can get access to them via this struct.
/// </summary>
public readonly ref struct ExtensionValues(ReadOnlySpan<char> line)
{
	public ReadOnlySpan<char> Value { get; } = line[^1] == ',' ? line[..^1] : line;

	public ExtensionValue this[string key]
	{
		get
		{
			var keyIndex = Value.IndexOf(key.AsSpan());
			if (keyIndex == -1)
			{
				return new ExtensionValue(ReadOnlySpan<char>.Empty);
			}

			var dataFollowingKey = Value[(keyIndex + key.Length + 1)..]; // adding the equal sign
			var potentiallyCommaSeparated = dataFollowingKey.IndexOf(',');
			return new ExtensionValue(potentiallyCommaSeparated == -1 ? dataFollowingKey : dataFollowingKey[..potentiallyCommaSeparated]);
		}
	}
}

public readonly ref struct ExtensionValue(ReadOnlySpan<char> val)
{
	private readonly ReadOnlySpan<char> value = val;

	public M3U8ValueKind ValueKind => value switch
	{
		{Length: 0} => M3U8ValueKind.Empty,
		['"', ..] => M3U8ValueKind.Quoted,
		_ => M3U8ValueKind.Unquoted
	};

	public ReadOnlySpan<char> Value => ValueKind == M3U8ValueKind.Quoted ? value[1..^1] : value;
}

public enum M3U8LineKind
{
	Empty,
	Source,
	Extension
}

public enum M3U8ValueKind
{
	Empty,
	Unquoted,
	Quoted
}