﻿using GalaSoft.MvvmLight;
using Emgu.CV.Structure;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using UnityEngine;
using GalaSoft.MvvmLight.CommandWpf;
using UniMoveStation.Model;
using System.Windows.Media;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using UniMoveStation.Service;
using UniMoveStation.View;
using UniMoveStation.Design;
using GalaSoft.MvvmLight.Messaging;
using UniMoveStation.Helper;
using System.ComponentModel;

namespace UniMoveStation.ViewModel
{
    public class SingleCameraViewModel : ViewModelBase
    {
        #region Member
        private RelayCommand<bool> _toggleCameraCommand;
        private RelayCommand<bool> _toggleTrackingCommand;
        private RelayCommand<bool> _toggleAnnotateCommand;
        private RelayCommand<ListBox> _enableTrackingCommand;

        public SingleCameraModel Camera
        {
            get;
            private set;
        }

        public ITrackerService TrackerService
        {
            get;
            private set;
        }

        public ICameraService CameraService
        {
            get;
            private set;
        }

        public IConsoleService ConsoleService
        {
            get;
            private set;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the MotionControllerViewModel class.
        /// </summary>
        [PreferredConstructor]
        public SingleCameraViewModel(SingleCameraModel camera, ITrackerService trackerService, ICameraService cameraService, IConsoleService consoleService)
        {
            TrackerService = trackerService;
            CameraService = cameraService;
            ConsoleService = consoleService;
            Camera = camera;

            CameraService.Initialize(Camera);
            TrackerService.Initialize(Camera);

            Messenger.Default.Register<AddMotionControllerMessage>(this,
                message =>
                {
                    if(!Camera.Controllers.Contains(message.MotionController))
                    {
                        Camera.Controllers.Add(message.MotionController);
                    }
                });

            Messenger.Default.Register<RemoveMotionControllerMessage>(this,
                message =>
                {
                    //TrackerService
                    message.MotionController.Tracking[Camera] = false;
                    Camera.Controllers.Remove(message.MotionController);
                });

            SimpleIoc.Default.Register(() => this, Camera.GUID, true);
            Messenger.Default.Send<AddCameraMessage>(new AddCameraMessage(Camera));
            ViewModelLocator.Instance.Navigation.CameraTabs.Add(this);
        }

        /// <summary>
        /// for design time purposes only
        /// </summary>
        public SingleCameraViewModel() : this(new SingleCameraModel(), new DesignTrackerService(),  new DesignCLEyeService(), new ConsoleService())
        {
            Camera.Name = "Design " + Camera.TrackerId;

#if DEBUG
            if (IsInDesignMode)
            {
                
            }
#endif
        }
        #endregion

        #region Dependency Properties
        /// <summary>
        /// The <see cref="CLEyeImageControlVisibility" /> property's name.
        /// </summary>
        public const string CLEyeImageControlVisibilityPropertyName = "CLEyeImageControlVisibility";

        private Visibility _clEyeImageControlVisibility = Visibility.Hidden;

        /// <summary>
        /// Sets and gets the CLEyeImageControlVisibility property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Visibility CLEyeImageControlVisibility
        {
            get
            {
                return _clEyeImageControlVisibility;
            }

            set
            {
                if (_clEyeImageControlVisibility == value)
                {
                    return;
                }

                _clEyeImageControlVisibility = value;
                RaisePropertyChanged(() => CLEyeImageControlVisibility);
            }
        }

        /// <summary>
        /// The <see cref="TrackerImageVisibility" /> property's name.
        /// </summary>
        public const string TrackerImageVisibilityPropertyName = "TrackerImageVisibility";

        private Visibility _trackerImageVisibility = Visibility.Hidden;

        /// <summary>
        /// Sets and gets the TrackerImageVisibility property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Visibility TrackerImageVisibility
        {
            get
            {
                return _trackerImageVisibility;
            }

            set
            {
                if (_trackerImageVisibility == value)
                {
                    return;
                }

                _trackerImageVisibility = value;
                RaisePropertyChanged(() => TrackerImageVisibility);
            }
        }
        #endregion

        #region Relay Commands
        /// <summary>
        /// Gets the AnnotateCommand.
        /// </summary>
        public RelayCommand<bool> ToggleAnnotateCommand
        {
            get
            {
                return _toggleAnnotateCommand
                    ?? (_toggleAnnotateCommand = new RelayCommand<bool>(DoToggleAnnotate));
            }
        }

        /// <summary>
        /// Gets the MyCommand.
        /// </summary>
        public RelayCommand<bool> ToggleCameraCommand
        {
            get
            {
                return _toggleCameraCommand
                    ?? (_toggleCameraCommand = new RelayCommand<bool>(DoToggleCamera));
            }
        }

        /// <summary>
        /// Gets the EnableTracking.
        /// </summary>
        public RelayCommand<bool> ToggleTrackingCommand
        {
            get
            {
                return _toggleTrackingCommand
                    ?? (_toggleTrackingCommand = new RelayCommand<bool>(DoToggleTracking));
            }
        }

        /// <summary>
        /// Gets the EnableTrackingCommand.
        /// </summary>
        public RelayCommand<ListBox> EnableTrackingCommand
        {
            get
            {
                return _enableTrackingCommand
                    ?? (_enableTrackingCommand = new RelayCommand<ListBox>(DoEnableTracking));
            }
        }
        #endregion

        #region Command Executions
        public void DoToggleAnnotate(bool annotate)
        {
            Camera.Annotate = annotate;
            ConsoleService.WriteLine("Annotate: " + annotate);
        }

        private void DoToggleCamera(bool enabled)
        {
            if (enabled)
            {
                if(TrackerService.Enabled)
                {
                    Camera.ShowImage = true;
                }
                else if(!CameraService.Enabled)
                {
                    Camera.ShowImage = CameraService.Start();
                }
            }
            else
            {
                if (CameraService.Enabled) Camera.ShowImage = CameraService.Stop();
                else if (TrackerService.Enabled) Camera.ShowImage = false;
            }
            ConsoleService.WriteLine("Show Image: " + Camera.ShowImage);
        }

        public void DoToggleTracking(bool enabled)
        {
            if (enabled)
            {
                if (CameraService.Enabled)
                {
                    CameraService.Stop();
                }
                Camera.Tracking = TrackerService.Start();
            }
            else
            {
                Camera.Tracking = TrackerService.Stop();
                if (Camera.ShowImage)
                {
                    CameraService.Start();
                }
            }
            ConsoleService.WriteLine("Tracking: " + enabled);
        }

        public void DoEnableTracking(ListBox listBox)
        {
            int index = -1;
            foreach(MotionControllerModel mc in listBox.Items)
            {
                index++;
                ListBoxItem listBoxItem = (ListBoxItem) listBox.ItemContainerGenerator.ContainerFromItem(mc);
                ContentPresenter contentPresenter = FindVisualChild<ContentPresenter>(listBoxItem);
                DataTemplate dataTemplate = contentPresenter.ContentTemplate;
                CheckBox checkBox = (CheckBox) dataTemplate.FindName("CheckBox", contentPresenter);
                bool isChecked = (bool) checkBox.IsChecked;
                if (isChecked)
                {
                    mc.Tracking[Camera] = true;
                }
                else
                {
                    mc.Tracking[Camera] = false;
                }
                ConsoleService.WriteLine(string.Format("Tracking ({0}): {1}", mc.Name, isChecked));
            }
        } // DoEnableTracking
        #endregion

        #region Misc
        public override void Cleanup()
        {
            TrackerService.Stop();
            CameraService.Stop();
            Messenger.Default.Send<RemoveCameraMessage>(new RemoveCameraMessage(Camera));
            SimpleIoc.Default.Unregister<SingleCameraModel>();
            base.Cleanup();
        }

        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/bb613579.aspx
        /// </summary>
        /// <typeparam name="childItem"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
        #endregion
    } // SingleCameraViewModel
} // Namespace