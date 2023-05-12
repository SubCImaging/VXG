//-----------------------------------------------------------------------
// <copyright file="RTSPService.cs" company="SubC Imaging">
// Copyright (c) SubC Imaging. All rights reserved.
// </copyright>
// <author>Adam Rowe</author>
//-----------------------------------------------------------------------
namespace SubCRayfin.Services
{
    using System;
    using System.Drawing;
    using System.Threading.Tasks;
    using Android.Hardware.Camera2;
    using Veg.Mediacapture.Sdk;
    using static Veg.Mediacapture.Sdk.MediaCaptureConfig;

    /// <summary>
    /// Responsible for streaming RTSP.
    /// </summary>
    public class RTSPService
    {
        private readonly CameraDevice cameraDevice;
        private readonly CaptureSessionService captureService;
        private readonly MediaCapture mediaCapture;

        /// <summary>
        /// Initializes a new instance of the <see cref="RTSPService" /> class.
        /// </summary>
        /// <param name="mediaCapture">Media capture used to encode.</param>
        /// <param name="cameraDevice">Camera device for generating requests.</param>
        /// <param name="captureService">Capture serivce to repeat the request.</param>
        public RTSPService(
            MediaCapture mediaCapture,
            CameraDevice cameraDevice,
            CaptureSessionService captureService)
        {
            this.mediaCapture = mediaCapture ?? throw new ArgumentNullException(nameof(mediaCapture));
            this.cameraDevice = cameraDevice ?? throw new ArgumentNullException(nameof(cameraDevice));
            this.captureService = captureService ?? throw new ArgumentNullException(nameof(captureService));
        }

        /// <summary>
        /// Start streaming with the default HD values.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
        public Task StartAsync() => StartAsync(new Size(1920, 1080), 25_000);

        private async Task StartAsync(Size resolution, int bitrate)
        {
            mediaCapture.StopStreaming();

            var mConfig = mediaCapture.Config;
            mConfig.Streaming = true;
            mConfig.PreviewScaleType = 0;
            mConfig.Transcoding = true;
            mConfig.CaptureMode = CaptureModes.PpModeVideo.Val();
            mConfig.StreamType = StreamerTypes.StreamTypeRtspServer.Val();
            mConfig.Url = "rtsp://@:" + 5540;

            var strResolution = $"{resolution.Width}x{resolution.Height}";

            switch (strResolution)
            {
                case "3840x2160":
                    mConfig.VideoResolution = MediaCaptureConfig.CaptureVideoResolution.VR3840x2160;
                    break;

                case "1920x1080":
                    mConfig.VideoResolution = MediaCaptureConfig.CaptureVideoResolution.VR1920x1080;
                    break;

                case "1280x720":
                    mConfig.VideoResolution = MediaCaptureConfig.CaptureVideoResolution.VR1280x720;
                    break;

                case "720x480":
                    mConfig.VideoResolution = MediaCaptureConfig.CaptureVideoResolution.VR720x480;
                    break;

                default:
                    break;
            }

            mConfig.CaptureSource = CaptureSources.PpModeExternal.Val();
            mConfig.VideoOrientation = MediaCaptureConfig.McOrientationLandscape;
            mConfig.VideoBitrate = bitrate;
            mConfig.VideoKeyFrameInterval = 1;
            mConfig.VideoBitrateMode = MediaCaptureConfig.BitrateModeVbr;
            mConfig.SetVideoTimestampType(MediaCaptureConfig.VideoTimestampType.Source);

            mediaCapture.Open(mConfig, null);

            var builder = cameraDevice.CreateCaptureRequest(CameraTemplate.Record);

            captureService.UpdateSurface(SubCTools.Droid.Enums.SurfaceTypes.RTSP, mediaCapture.Surface, builder);
            await captureService.RepeatAsync();

            mediaCapture.StartStreaming();
        }
    }
}