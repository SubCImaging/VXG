//-----------------------------------------------------------------------
// <copyright file="MediaCaptureCallback.cs" company="SubC Imaging">
// Copyright (c) SubC Imaging. All rights reserved.
// </copyright>
// <author>Aaron Watson</author>
//-----------------------------------------------------------------------

namespace SubC.Grenadier.RTSP
{
    using Java.Nio;
    using Veg.Mediacapture.Sdk;
    using static Veg.Mediacapture.Sdk.MediaCapture;

    /// <summary>
    /// Media capture callback class.
    /// </summary>
    public class MediaCaptureCallback : Java.Lang.Object, IMediaCaptureCallback
    {
        /// <summary>
        /// Logs the information from the OnCaptureReceiveData.
        /// </summary>
        /// <param name="buffer">The <see cref="ByteBuffer" /> that contains the data.</param>
        /// <param name="type">The type of data.</param>
        /// <param name="size">The size of the data.</param>
        /// <param name="pts">The total data of the stream.</param>
        /// <returns>Return code.</returns>
        public int OnCaptureReceiveData(ByteBuffer buffer, int type, int size, long pts)
        {
            return 0;
        }

        /// <summary>
        /// The method that gets called when the capture status changes.
        /// </summary>
        /// <param name="arg">The <see cref="CaptureNotifyCodes" /> status.</param>
        /// <returns>Return code.</returns>
        public int OnCaptureStatus(int arg)
        {
            return 0;
        }
    }
}