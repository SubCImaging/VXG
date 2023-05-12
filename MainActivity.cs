// <copyright file="MainActivity.cs" company="SubC Imaging">
//     Copyright (c) SubC Imaging. All rights reserved.
// </copyright>

namespace SubC.Viperfish
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Android.App;
    using Android.Hardware.Camera2;
    using Android.Hardware.Camera2.Params;
    using Android.OS;
    using Android.Runtime;
    using Android.Views;
    using AndroidX.AppCompat.App;
    using EmbedIO;
    using EmbedIO.WebApi;
    using Microsoft.AspNetCore.SignalR.Client;
    using Plugin.Permissions;
    using SubCRayfin.Services;
    using SubCTools.Droid;
    using Veg.Mediacapture.Sdk;
    using Xamarin.Essentials;
    using static Veg.Mediacapture.Sdk.MediaCaptureConfig;
    using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;
    using Range = Android.Util.Range;

    /// <summary>
    /// Responsible for launching the application.
    /// </summary>
    [Activity(Name = "subcviperfish.activity", Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private CameraDevice camera;
        private MediaCapture capturer;
        private AutoFitTextureView preview;
        private RTSPService rtspService;

        /// <inheritdoc />
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        /// <inheritdoc />
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            Window.AddFlags(WindowManagerFlags.Fullscreen);

            RequestWindowFeature(WindowFeatures.NoTitle);

            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(SubC_Viperfish.Resource.Layout.activity_main);

            var status = await CrossPermissions.Current.CheckPermissionStatusAsync<CameraPermission>();
            if (status != PermissionStatus.Granted)
            {
                status = await CrossPermissions.Current.RequestPermissionAsync<CameraPermission>();
            }

            status = await CrossPermissions.Current.CheckPermissionStatusAsync<MicrophonePermission>();
            if (status != PermissionStatus.Granted)
            {
                status = await CrossPermissions.Current.RequestPermissionAsync<MicrophonePermission>();
            }

            // get the preview to display video
            preview = FindViewById<AutoFitTextureView>(SubC_Viperfish.Resource.Id.Preview);

            while (!preview.IsAvailable)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }

            await InitRTSP();
        }

        private async Task InitRTSP()
        {
            capturer = new MediaCapture(this, null);

            var mConfig = capturer.Config;
            mConfig.CaptureSource = CaptureSources.PpModeExternal.Val();

            mConfig.SetVideoTimestampType(MediaCaptureConfig.VideoTimestampType.Source);

            MediaCapture.RequestPermission(this, mConfig.CaptureSource);

            capturer.Open(mConfig, null);

            var cameraManager = (CameraManager)GetSystemService(Android.Content.Context.CameraService);

            // get the camera from the camera manager with the given ID
            camera = await SubCTools.Droid.Helpers.Camera.OpenCameraAsync("0", cameraManager);

            var captureService = new CaptureSessionService(camera);

            var p = new PreviewService(preview, camera, captureService);
            await p.StartAsync();

            rtspService = new RTSPService(capturer, camera, captureService);
            await rtspService.StartAsync();
        }

    }
}