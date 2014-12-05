﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using UniMoveStation.Model;
using UniMoveStation.Nito;
using UniMoveStation.ViewModel.Flyout;

namespace UniMoveStation.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class NavigationViewModel : ViewModelBase
    {
        private int _lastSelectedIndex = 0;
        private int _motionControllerCount = 0;
        private int _cameraCount = 0;
        private RelayCommand<TabControl> _tabSelectedCommand;
        private RelayCommand<Object> _addCommand;

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the NavigationViewModel class.
        /// </summary>
        [PreferredConstructor]
        public NavigationViewModel()
        {
            CameraTabs = new ObservableCollection<object>();
            CameraTabs.CollectionChanged += (obj, args) =>
            {
                CameraCount = Math.Max(CameraTabs.Count - 1, 0);
            };
            MotionControllerTabs = new ObservableCollection<object>();
            MotionControllerTabs.CollectionChanged += (obj, args) =>
            {
                MotionControllerCount = Math.Max(MotionControllerTabs.Count - 1, 0);
            };

            ServerTabs = new ObservableCollection<object>();

            Refresh();
        }
        #endregion

        #region Properties
        public int MotionControllerCount
        {
            get { return _motionControllerCount; }
            set { Set(() => MotionControllerCount, ref _motionControllerCount, value); }
        }

        public int CameraCount
        {
            get { return _cameraCount; }
            set { Set(() => CameraCount, ref _cameraCount, value); }
        }

        public ObservableCollection<object> CameraTabs
        {
            get;
            private set;
        }

        public ObservableCollection<object> MotionControllerTabs
        {
            get;
            private set;
        }

        public ObservableCollection<object> ServerTabs
        {
            get;
            private set;
        }
        #endregion

        #region Relay Commands
        /// <summary>
        /// Gets the TabSelectedCommand.
        /// </summary>
        public RelayCommand<TabControl> TabSelectedCommand
        {
            get
            {
                return _tabSelectedCommand
                    ?? (_tabSelectedCommand = new RelayCommand<TabControl>(
                    tabControl =>
                    {
                        if(tabControl.SelectedIndex == tabControl.Items.Count - 1)
                        {
                            tabControl.SelectedIndex = _lastSelectedIndex;
                        }
                        else
                        {
                            _lastSelectedIndex = tabControl.SelectedIndex;
                        }
                    }));
            }
        }

        /// <summary>
        /// Gets the AddCommand.
        /// </summary>
        public RelayCommand<Object> AddCommand
        {
            get
            {
                return _addCommand
                    ?? (_addCommand = new RelayCommand<Object>(
                    tag =>
                    {
                        if (tag.ToString().Equals("controllers"))
                        {
                            AddMotionControllerViewModel amcvm = ViewModelLocator.Instance.AddMotionController;
                            SimpleIoc.Default.GetInstance<MainViewModel>().DoToggleFlyout(amcvm);
                            amcvm.DoRefresh();
                        }
                        else if (tag.ToString().Equals("cameras"))
                        {
                            AddCameraViewModel acvm = ViewModelLocator.Instance.AddCamera;
                            SimpleIoc.Default.GetInstance<MainViewModel>().DoToggleFlyout(acvm);
                            acvm.DoRefresh();
                        }
                        else if (tag.ToString().Equals("server"))
                        {
                            if(SimpleIoc.Default.GetInstance<ServerViewModel>().Server.Enabled)
                            {
                                NitoClient client = new NitoClient();
                                client.connect("127.0.0.1", 3000);
                            }
                        }
                    }));
            }
        }

        private RelayCommand<object> _removeCommand;

        /// <summary>
        /// Gets the RemoveCommand.
        /// </summary>
        public RelayCommand<object> RemoveCommand
        {
            get
            {
                return _removeCommand
                    ?? (_removeCommand = new RelayCommand<object>(DoRemove));
            }
        }

        public void DoRemove(object obj)
        {
            if(obj is MotionControllerViewModel)
            {
                MotionControllerViewModel mcvw = (MotionControllerViewModel) obj;
                mcvw.Cleanup();
            }
            else if (obj is SingleCameraViewModel)
            {
                SingleCameraViewModel scvw = (SingleCameraViewModel) obj;
                scvw.Cleanup();
            }
        }
        #endregion

        public void Refresh()
        {
            {
                CameraTabs.Clear();
                CameraTabs.Add(SimpleIoc.Default.GetInstance<CamerasViewModel>());
                foreach (SingleCameraViewModel scvm in SimpleIoc.Default.GetAllCreatedInstances<SingleCameraViewModel>())
                {
                    CameraTabs.Add(scvm);
                }
                IEditableCollectionView itemsView = (IEditableCollectionView)CollectionViewSource.GetDefaultView(CameraTabs);
                itemsView.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;
            }

            {
                MotionControllerTabs.Clear();
                // TODO replace with mc
                MotionControllerTabs.Add(SimpleIoc.Default.GetInstance<CamerasViewModel>());
                foreach (MotionControllerViewModel mcvm in SimpleIoc.Default.GetAllCreatedInstances<MotionControllerViewModel>())
                {
                    MotionControllerTabs.Add(mcvm);
                }
                IEditableCollectionView itemsView = (IEditableCollectionView)CollectionViewSource.GetDefaultView(MotionControllerTabs);
                itemsView.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;
            }

            {
                ServerTabs.Clear();
                ServerTabs.Add(SimpleIoc.Default.GetInstance<ServerViewModel>());
                IEditableCollectionView itemsView = (IEditableCollectionView)CollectionViewSource.GetDefaultView(ServerTabs);
                itemsView.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;
            }
        } // Refresh
    } // NavigationViewModel
} // namespace