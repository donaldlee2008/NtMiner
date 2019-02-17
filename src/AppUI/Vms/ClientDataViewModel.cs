﻿using NTMiner.MinerServer;
using System;

namespace NTMiner.Vms {
    public class ClientDataViewModel : ViewModelBase, IClientData {
        private readonly ClientData _data;
        public ClientDataViewModel(ClientData data) {
            _data = data;
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _data.Id;
            set {
                if (_data.Id != value) {
                    _data.Id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public Guid WorkId {
            get => _data.WorkId;
            set {
                if (_data.WorkId != value) {
                    _data.WorkId = value;
                    OnPropertyChanged(nameof(WorkId));
                    OnPropertyChanged(nameof(SelectedMineWork));
                }
            }
        }

        public MineWorkViewModel SelectedMineWork {
            get {
                if (WorkId == Guid.Empty) {
                    return MineWorkViewModel.FreeMineWork;
                }
                MineWorkViewModel vm;
                if (MineWorkViewModels.Current.TryGetMineWorkVm(WorkId, out vm)) {
                    return vm;
                }
                return MineWorkViewModel.FreeMineWork;
            }
            set {
                if (WorkId != value.Id) {
                    WorkId = value.Id;
                    OnPropertyChanged(nameof(SelectedMineWork));
                    Server.ControlCenterService.UpdateClientAsync(this.Id, nameof(IClientData.WorkId), value.Id, null);
                }
            }
        }

        public string WorkName {
            get {
                if (this.WorkId == Guid.Empty) {
                    return "自由作业";
                }
                IMineWork mineWork;
                if (NTMinerRoot.Current.MineWorkSet.TryGetMineWork(this.WorkId, out mineWork)) {
                    return mineWork.Name;
                }
                return "未知作业";
            }
        }

        public string Version {
            get => _data.Version;
            set {
                if (_data.Version != value) {
                    _data.Version = value;
                    OnPropertyChanged(nameof(Version));
                }
            }
        }

        public DateTime ModifiedOn {
            get => _data.ModifiedOn;
            set {
                if (_data.ModifiedOn != value) {
                    _data.ModifiedOn = value;
                    OnPropertyChanged(nameof(ModifiedOn));
                    OnPropertyChanged(nameof(ModifiedOnText));
                    OnPropertyChanged(nameof(LastActivedOnText));
                    OnPropertyChanged(nameof(IsClientOnline));
                    OnPropertyChanged(nameof(IsMining));
                }
            }
        }

        public bool IsClientOnline {
            get {
                return this.ModifiedOn.AddSeconds(121) >= DateTime.Now;
            }
        }

        public string ModifiedOnText {
            get {
                return this.ModifiedOn.ToString("HH:mm:ss");
            }
        }

        public string LastActivedOnText {
            get {
                TimeSpan timeSpan = DateTime.Now - ModifiedOn;
                if (timeSpan.Days >= 1) {
                    return "一天前";
                }
                if (timeSpan.Hours > 0) {
                    return timeSpan.Hours + "小时前";
                }
                if (timeSpan.Minutes > 0) {
                    return timeSpan.Minutes + "分钟前";
                }
                return timeSpan.Seconds + "秒前";
            }
        }

        public bool IsMining {
            get {
                if (!IsClientOnline) {
                    return false;
                }
                return _data.IsMining;
            }
            set {
                if (_data.IsMining != value) {
                    _data.IsMining = value;
                    OnPropertyChanged(nameof(IsMining));
                }
            }
        }

        public string MinerName {
            get => _data.MinerName;
            set {
                if (_data.MinerName != value) {
                    var old = _data.MinerName;
                    _data.MinerName = value;
                    OnPropertyChanged(nameof(MinerName));
                    Server.ControlCenterService.UpdateClientAsync(this.Id, nameof(MinerName), value, response1 => {
                        if (response1.IsSuccess()) {
                            MinerClientService.Instance.SetMinerProfilePropertyAsync(this.MinerIp, nameof(MinerName), value, response2 => {
                                if (!response2.IsSuccess()) {
                                    _data.MinerName = old;
                                    Write.UserLine($"{this.MinerIp} {response2?.Description}", ConsoleColor.Red);
                                }
                            });
                        }
                        else {
                            _data.MinerName = old;
                        }
                    });
                }
            }
        }

        public Guid GroupId {
            get { return _data.GroupId; }
            set {
                if (_data.GroupId != value) {
                    _data.GroupId = value;
                    OnPropertyChanged(nameof(GroupId));
                    OnPropertyChanged(nameof(SelectedMinerGroup));
                }
            }
        }

        private MinerGroupViewModel _selectedMinerGroup;
        public MinerGroupViewModel SelectedMinerGroup {
            get {
                if (_selectedMinerGroup == null || _selectedMinerGroup.Id != GroupId) {
                    MinerGroupViewModels.Current.TryGetMineWorkVm(GroupId, out _selectedMinerGroup);
                    if (_selectedMinerGroup == null) {
                        _selectedMinerGroup = MinerGroupViewModel.PleaseSelect;
                    }
                }
                return _selectedMinerGroup;
            }
            set {
                if (_selectedMinerGroup != value) {
                    var old = _selectedMinerGroup;
                    _selectedMinerGroup = value;
                    try {
                        Server.ControlCenterService.UpdateClientAsync(
                            this.Id, nameof(GroupId), value.Id, response => {
                                if (!response.IsSuccess()) {
                                    this.GroupId = old.Id;
                                    Write.UserLine($"{this.MinerIp} {response?.Description}", ConsoleColor.Red);
                                }
                                else {
                                    this.GroupId = value.Id;
                                    OnPropertyChanged(nameof(SelectedMinerGroup));
                                }
                            });
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                }
            }
        }

        private long _mainCoinSpeed;
        private long _dualCoinSpeed;

        public string MinerIp {
            get => _data.MinerIp;
            set {
                if (_data.MinerIp != value) {
                    _data.MinerIp = value;
                    OnPropertyChanged(nameof(MinerIp));
                }
            }
        }

        public string MainCoinCode {
            get => _data.MainCoinCode ?? string.Empty;
            set {
                if (_data.MainCoinCode != value) {
                    _data.MainCoinCode = value;
                    OnPropertyChanged(nameof(MainCoinCode));
                }
            }
        }

        public CoinViewModel MainCoinVm {
            get {
                CoinViewModel coinVm;
                CoinViewModels.Current.TryGetCoinVm(this.MainCoinCode, out coinVm);
                return coinVm;
            }
        }

        public long MainCoinSpeed {
            get => _mainCoinSpeed;
            set {
                _mainCoinSpeed = value;
                OnPropertyChanged(nameof(MainCoinSpeed));
                OnPropertyChanged(nameof(MainCoinSpeedText));
            }
        }

        public string MainCoinSpeedText {
            get {
                return this.MainCoinSpeed.ToUnitSpeedText();
            }
        }

        public string MainCoinPool {
            get => _data.MainCoinPool;
            set {
                if (_data.MainCoinPool != value) {
                    _data.MainCoinPool = value;
                    OnPropertyChanged(nameof(MainCoinPool));
                }
            }
        }

        public string MainCoinWallet {
            get => _data.MainCoinWallet;
            set {
                if (_data.MainCoinWallet != value) {
                    _data.MainCoinWallet = value;
                    OnPropertyChanged(nameof(MainCoinWallet));
                }
            }
        }

        public string Kernel {
            get => _data.Kernel;
            set {
                if (_data.Kernel != value) {
                    _data.Kernel = value;
                    OnPropertyChanged(nameof(Kernel));
                }
            }
        }

        public bool IsDualCoinEnabled {
            get => _data.IsDualCoinEnabled;
            set {
                if (_data.IsDualCoinEnabled != value) {
                    _data.IsDualCoinEnabled = value;
                    OnPropertyChanged(nameof(IsDualCoinEnabled));
                }
            }
        }

        public string DualCoinCode {
            get => _data.DualCoinCode ?? string.Empty;
            set {
                if (_data.DualCoinCode != value) {
                    _data.DualCoinCode = value;
                    OnPropertyChanged(nameof(DualCoinCode));
                }
            }
        }

        public CoinViewModel DualCoinVm {
            get {
                CoinViewModel coinVm;
                CoinViewModels.Current.TryGetCoinVm(this.DualCoinCode, out coinVm);
                return coinVm;
            }
        }

        public long DualCoinSpeed {
            get => _dualCoinSpeed;
            set {
                _dualCoinSpeed = value;
                OnPropertyChanged(nameof(DualCoinSpeed));
                OnPropertyChanged(nameof(DualCoinSpeedText));
            }
        }

        public string DualCoinSpeedText {
            get {
                return this.DualCoinSpeed.ToUnitSpeedText();
            }
        }

        public string DualCoinPool {
            get => _data.DualCoinPool;
            set {
                if (_data.DualCoinPool != value) {
                    _data.DualCoinPool = value;
                    OnPropertyChanged(nameof(DualCoinPool));
                }
            }
        }

        public string DualCoinWallet {
            get => _data.DualCoinWallet;
            set {
                if (_data.DualCoinWallet != value) {
                    _data.DualCoinWallet = value;
                    OnPropertyChanged(nameof(DualCoinWallet));
                }
            }
        }

        public string GpuInfo {
            get => _data.GpuInfo;
            set {
                if (_data.GpuInfo != value) {
                    _data.GpuInfo = value;
                    OnPropertyChanged(nameof(GpuInfo));
                }
            }
        }
    }
}
