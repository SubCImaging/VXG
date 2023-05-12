// <copyright file="CaptureSessionService.cs" company="SubC Imaging">
// Copyright (c) SubC Imaging. All rights reserved.
// </copyright>

namespace SubCRayfin.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Android.App;
    using Android.Content;
    using Android.Hardware.Camera2;
    using Android.OS;
    using Android.Runtime;
    using Android.Views;
    using Android.Widget;
    using SubCTools.Droid.Enums;
    using SubCTools.Droid.Helpers;

    /// <summary>
    /// Responsible for maintaining the surfaces, and requests.
    /// </summary>
    public class CaptureSessionService
    {
        private readonly CameraDevice cameraDevice;

        private readonly Dictionary<SurfaceTypes, (Surface, CaptureRequest.Builder)> repeatingSurfaces = new Dictionary<SurfaceTypes, (Surface, CaptureRequest.Builder)>();
        private CameraCaptureSession captureSession;

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptureSessionService"/> class.
        /// </summary>
        /// <param name="cameraDevice">Camera device for generating capture sessions.</param>
        public CaptureSessionService(CameraDevice cameraDevice)
        {
            this.cameraDevice = cameraDevice;
        }

        /// <summary>
        /// Capture the request.
        /// </summary>
        /// <param name="request">Request to capture.</param>
        public void Capture(CaptureRequest request)
        {
            captureSession?.Capture(request, null, null);
        }

        /// <summary>
        /// Remove the given surface type from the session.
        /// </summary>
        /// <param name="surfaceType">Surface type to remove.</param>
        public void RemoveSurface(SurfaceTypes surfaceType)
        {
            repeatingSurfaces.Remove(surfaceType);
        }

        /// <summary>
        /// Repeat on the existing surfaces.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Throws when you don't have any surfaces.</exception>
        public async Task RepeatAsync()
        {
            if (!repeatingSurfaces.Any())
            {
                throw new Exception("You must have at least one surface to request a capture.");
            }

            captureSession?.StopRepeating();
            captureSession?.Close();
            captureSession = await cameraDevice.CreateCaptureSessionAsync(repeatingSurfaces.Values.Select(r => r.Item1).ToArray());

            // select the highest priority builder
            var key = repeatingSurfaces.Keys.OrderByDescending(c => c).First();

            var builder = repeatingSurfaces[key].Item2;

            foreach (var item in repeatingSurfaces)
            {
                if (item.Key == SurfaceTypes.Still)
                {
                    continue;
                }

                builder.AddTarget(item.Value.Item1);
            }

            captureSession.SetRepeatingRequest(builder.Build(), null, null);
        }

        /// <summary>
        /// Update the given surface and builder for the surface stype.
        /// </summary>
        /// <param name="surfaceType">Surface type to update.</param>
        /// <param name="surface">Surface to update.</param>
        /// <param name="builder">Builder to update.</param>
        public void UpdateSurface(
            SurfaceTypes surfaceType,
            Surface surface,
            CaptureRequest.Builder builder)
        {
            if (repeatingSurfaces.ContainsKey(surfaceType))
            {
                repeatingSurfaces[surfaceType] = (surface, builder);
            }
            else
            {
                repeatingSurfaces.Add(surfaceType, (surface, builder));
            }
        }
    }
}