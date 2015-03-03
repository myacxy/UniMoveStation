﻿using Emgu.CV;
using Emgu.CV.Structure;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using UniMoveStation.View;
using GalaSoft.MvvmLight.CommandWpf;
using UniMoveStation.Model;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Threading;
using UniMoveStation.Helper;
using UniMoveStation.ViewModel;
using System.Collections.ObjectModel;
using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace UniMoveStation.Service
{
    public class CameraPositioningCalibrationService
    {
        private BaseMetroDialog _dialog;
        private MetroWindow _owningWindow;
        private RelayCommand _cancelCommand;
        private RelayCommand _applyCommand;
        private RelayCommand _saveCommand;

        private ObservableCollection<Visual3D> _helixItems;
        private ObservableCollection<SingleCameraViewModel> _cameras;

        public ObservableCollection<SingleCameraViewModel> Cameras
        {
            get
            {
                if (_cameras == null)
                {
                    _cameras = new ObservableCollection<SingleCameraViewModel>();
                }
                return _cameras;
            }
            private set
            {
                _cameras = value;
            }
        }

        public ObservableCollection<Visual3D> HelixItems
        {
            get
            {
                if (_helixItems == null)
                {
                    _helixItems = new ObservableCollection<Visual3D>();

                    InitializeHelix();
                }
                return _helixItems;
            }
        }

        public bool InputAnglesManually
        {
            get;
            set;
        }

        #region Constructor
        public CameraPositioningCalibrationService(ObservableCollection<SingleCameraViewModel> cameras)
        {
            Cameras = cameras;
        }
        #endregion

        #region Commands
        /// <summary>
        /// Gets the CancelCommand.
        /// </summary>
        public RelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand
                    ?? (_cancelCommand = new RelayCommand(DoCancel));
            }
        }

        /// <summary>
        /// Gets the ApplyCommand.
        /// </summary>
        public RelayCommand ApplyCommand
        {
            get
            {
                return _applyCommand
                    ?? (_applyCommand = new RelayCommand(DoApply));
            }
        }

        /// <summary>
        /// Gets the SaveCommand.
        /// </summary>
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand
                    ?? (_saveCommand = new RelayCommand(DoSave));
            }
        }
        #endregion

        #region Command Executions
        public async void DoCancel()
        {
            await _dialog.RequestCloseAsync();
        }

        public void DoApply()
        {
            _helixItems.Clear();

            InitializeHelix();

            if(Cameras.Count == 4 && !InputAnglesManually)
            {
                {
                    double a = Math.Abs(GetCamera(0).Calibration.TranslationVector[0, 0]) + Math.Abs(GetCamera(2).Calibration.TranslationVector[0, 0]);
                    double b = Math.Abs(GetCamera(0).Calibration.TranslationVector[2, 0]) + Math.Abs(GetCamera(2).Calibration.TranslationVector[2, 0]);
                    double c = Math.Sqrt(a * a + b * b);
                    double q = (a * a) / c;
                    double p = c - q;
                    double h = Math.Sqrt(p * q);
                    double alpha = (180 / Math.PI) * Math.Atan(h / p);
                    double beta = 90 - alpha;

                    GetCamera(0).Calibration.YAngle = (float)alpha;
                    GetCamera(1).Calibration.YAngle = (float)-alpha;
                    GetCamera(2).Calibration.YAngle = (float)(-180 + alpha);
                    GetCamera(3).Calibration.YAngle = (float)(180 - alpha);

                    GetCamera(0).Calibration.RotationMatrix = Utilities.Utils.GetYRotationMatrix(GetCamera(0).Calibration.YAngle);
                    GetCamera(1).Calibration.RotationMatrix = Utilities.Utils.GetYRotationMatrix(GetCamera(1).Calibration.YAngle);
                    GetCamera(2).Calibration.RotationMatrix = Utilities.Utils.GetYRotationMatrix(GetCamera(2).Calibration.YAngle);
                    GetCamera(3).Calibration.RotationMatrix = Utilities.Utils.GetYRotationMatrix(GetCamera(3).Calibration.YAngle);
                }
                {
                    double a = GetCamera(0).Calibration.TranslationVector[1, 0] - GetCamera(2).Calibration.TranslationVector[1, 0];
                    double ax = Math.Abs(GetCamera(0).Calibration.TranslationVector[0, 0]) + Math.Abs(GetCamera(2).Calibration.TranslationVector[0, 0]);
                    double az = Math.Abs(GetCamera(0).Calibration.TranslationVector[2, 0]) + Math.Abs(GetCamera(2).Calibration.TranslationVector[2, 0]);
                    double c = Math.Sqrt(ax * ax + az * az);
                    double q = (a * a) / c;
                    double p = c - q;
                    double h = Math.Sqrt(p * q);
                    double alpha = (180 / Math.PI) * Math.Atan(h / p);
                    double beta = 90 - alpha;

                    if (GetCamera(0).Calibration.TranslationVector[1, 0] < GetCamera(2).Calibration.TranslationVector[1, 0])
                    {
                        GetCamera(0).Calibration.XAngle = (float)alpha;
                        GetCamera(2).Calibration.XAngle = (float)-alpha;
                    }
                    else
                    {
                        GetCamera(0).Calibration.XAngle = (float)-alpha;
                        GetCamera(2).Calibration.XAngle = (float)alpha;
                    }
                }
                {
                    double a = GetCamera(1).Calibration.TranslationVector[1, 0] - GetCamera(3).Calibration.TranslationVector[1, 0];
                    double ax = Math.Abs(GetCamera(1).Calibration.TranslationVector[0, 0]) + Math.Abs(GetCamera(3).Calibration.TranslationVector[0, 0]);
                    double az = Math.Abs(GetCamera(1).Calibration.TranslationVector[2, 0]) + Math.Abs(GetCamera(3).Calibration.TranslationVector[2, 0]);
                    double c = Math.Sqrt(ax * ax + az * az);
                    double q = (a * a) / c;
                    double p = c - q;
                    double h = Math.Sqrt(p * q);
                    double alpha = (180 / Math.PI) * Math.Atan(h / p);
                    double beta = 90 - alpha;
                    

                    if (GetCamera(1).Calibration.TranslationVector[1, 0] < GetCamera(3).Calibration.TranslationVector[1, 0])
                    {
                        GetCamera(1).Calibration.XAngle = (float)alpha;
                        GetCamera(3).Calibration.XAngle = (float)-alpha;
                    }
                    else
                    {
                        GetCamera(1).Calibration.XAngle = (float)-alpha;
                        GetCamera(3).Calibration.XAngle = (float)alpha;
                    }
                }
            }
        }

        private SingleCameraModel GetCamera(int position)
        {
            foreach(SingleCameraViewModel scvm in Cameras)
            {
                if (scvm.Camera.Calibration.Position == position) return scvm.Camera;
            }
            return null;
        }

        public void DoSave()
        {
            foreach (SingleCameraViewModel scvm in Cameras)
            {
                ViewModelLocator.Instance.Settings.DoSaveCalibration(scvm.Camera);
            }
            DoCancel();
        }
        #endregion

        public async void ShowMetroDialog(MetroWindow window)
        {
            _dialog = new CameraPositioningCalibrationView(window);
            _dialog.DataContext = this;
            _owningWindow = window;

            await _owningWindow.ShowMetroDialogAsync(_dialog);
        }

        private void InitializeHelix()
        {
            _helixItems.Add(new HelixToolkit.Wpf.SunLight());
            _helixItems.Add(new HelixToolkit.Wpf.GridLinesVisual3D()
            {
                Width = 500,
                Length = 500
            });
            RectangleVisual3D rec = new RectangleVisual3D();

            foreach (SingleCameraViewModel scvm in Cameras)
            {
                CubeVisual3D cube = new CubeVisual3D();
                cube.SideLength = 10;
                cube.Fill = new SolidColorBrush(Colors.Blue);
                cube.Center = new Point3D(scvm.Camera.Calibration.TranslationVector[0, 0],
                    scvm.Camera.Calibration.TranslationVector[2, 0],
                    scvm.Camera.Calibration.TranslationVector[1, 0]);
                _helixItems.Add(cube);
            }

            ArrowVisual3D arrow = new ArrowVisual3D()
            {
                Point1 = new Point3D(
                    GetCamera(0).Calibration.TranslationVector[0, 0],
                    GetCamera(0).Calibration.TranslationVector[2, 0],
                    GetCamera(0).Calibration.TranslationVector[1, 0]),
                Point2 = new Point3D(
                    GetCamera(2).Calibration.TranslationVector[0, 0],
                    GetCamera(2).Calibration.TranslationVector[2, 0],
                    GetCamera(2).Calibration.TranslationVector[1, 0]),
                Fill = new SolidColorBrush(Colors.Yellow)
            };
            _helixItems.Add(arrow);

            arrow = new ArrowVisual3D()
            {
                Point1 = new Point3D(
                    GetCamera(1).Calibration.TranslationVector[0, 0],
                    GetCamera(1).Calibration.TranslationVector[2, 0],
                    GetCamera(1).Calibration.TranslationVector[1, 0]),
                Point2 = new Point3D(
                    GetCamera(3).Calibration.TranslationVector[0, 0],
                    GetCamera(3).Calibration.TranslationVector[2, 0],
                    GetCamera(3).Calibration.TranslationVector[1, 0]),
                Fill = new SolidColorBrush(Colors.Yellow)
            };
            _helixItems.Add(arrow);

            ArrowVisual3D axis = new ArrowVisual3D()
            {
                Origin = new Point3D(0, 0, 0),
                Direction = new Vector3D(250, 0, 0),
                Diameter = 2,
                Fill = new SolidColorBrush(Colors.Red)
            };
            _helixItems.Add(axis);

            axis = new ArrowVisual3D()
            {
                Origin = new Point3D(0, 0, 0),
                Direction = new Vector3D(0, 250, 0),
                Diameter = 2,
                Fill = new SolidColorBrush(Colors.Green)
            };
            _helixItems.Add(axis);

            axis = new ArrowVisual3D()
            {
                Origin = new Point3D(0, 0, 0),
                Direction = new Vector3D(0, 0, 250),
                Diameter = 2,
                Fill = new SolidColorBrush(Colors.Blue)
            };
            _helixItems.Add(axis);
        }
    } // CameraCalibrationService
} // namespace