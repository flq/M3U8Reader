namespace M3U8Reader;

public static class M3U8Extensions
{
    /// <summary>
    ///     Informs about a specific video stream. The next value is the URI of the stream.
    /// </summary>
    public const string ExtStreamInf = "EXT-X-STREAM-INF";

    /// <summary>
    ///     Gives access to e.g. duration information for single segments
    /// </summary>
    public const string ExtInf = "EXTINF";

    public static class ValueKeys
    {
            /// <summary>
        ///     In an extension of type <see cref="M3U8Extensions.ExtStreamInf" />, this key gives you access to the resolution of
        ///     the video.
        /// </summary>
        public const string Resolution = "RESOLUTION";
    }
}