//-----------------------------------------------------------------------
// <copyright file="PreviewService.cs" company="SubC Imaging">
// Copyright (c) SubC Imaging. All rights reserved.
// </copyright>
// <author>Adam Rowe</author>
//-----------------------------------------------------------------------
namespace SubCRayfin.Services
{
    using Android.Hardware.Camera2;
    using Android.Views;
    using System;
    using System.Threading.Tasks;
    using SubCTools.Droid;
    using SubCRayfin.Services;

    /// <summary>
    /// Responsible for starting the camera preview.
    /// </summary>
    public class PreviewService
    {
        private readonly CameraDevice cameraDevice;
        private readonly CaptureSessionService captureSessionService;
        private readonly AutoFitTextureView texture;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewService"/> class.
        /// </summary>
        /// <param name="texture">Texture to show the preview.</param>
        /// <param name="cameraDevice">Camera to build request.</param>
        /// <param name="captureSessionService">Used to repeat request.</param>
        public PreviewService(
            AutoFitTextureView texture,
            CameraDevice cameraDevice,
            CaptureSessionService captureSessionService)
        {
            this.texture = texture ?? throw new ArgumentNullException(nameof(texture));
            this.cameraDevice = cameraDevice ?? throw new ArgumentNullException(nameof(cameraDevice));
            this.captureSessionService = captureSessionService ?? throw new ArgumentNullException(nameof(captureSessionService));
        }

        /// <summary>
        /// Start previewing.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task StartAsync()
        {
            captureSessionService.UpdateSurface(SubCTools.Droid.Enums.SurfaceTypes.Preview, new Surface(texture.SurfaceTexture), cameraDevice.CreateCaptureRequest(CameraTemplate.Preview));
            await captureSessionService.RepeatAsync();
        }
    }
}