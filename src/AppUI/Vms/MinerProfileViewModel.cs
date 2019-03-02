﻿using NTMiner.Core;
using NTMiner.MinerServer;
using NTMiner.Profile;
using System;
using System.Linq;

namespace NTMiner.Vms {
    public class MinerProfileViewModel : ViewModelBase, IMinerProfile {
        public static readonly MinerProfileViewModel Current = new MinerProfileViewModel();

        private MinerProfileViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            VirtualRoot.On<MinerProfilePropertyChangedEvent>(
                "MinerProfile设置变更后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    OnPropertyChanged(message.PropertyName);
                });
            VirtualRoot.On<MineWorkPropertyChangedEvent>(
                "MineWork设置变更后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    OnPropertyChanged(message.PropertyName);
                });

            VirtualRoot.Accept<RefreshArgsAssemblyCommand>(
                "刷新参数总成",
                LogEnum.Console,
                action: cmd => {
                    this.OnPropertyChanged(nameof(this.ArgsAssembly));
                });
            VirtualRoot.On<MinerProfileSwichedEvent>(
                "MinerProfile切换后刷新Vm内存",
                LogEnum.Console,
                action: message => {
                    foreach (var propertyInfo in this.GetType().GetProperties()) {
                        this.OnPropertyChanged(propertyInfo.Name);
                    }
                });
        }

        public IMineWork MineWork {
            get {
                return NTMinerRoot.Current.MinerProfile.MineWork;
            }
        }

        public Guid Id {
            get { return NTMinerRoot.Current.MinerProfile.GetId(); }
        }

        public Guid GetId() {
            return this.Id;
        }

        public string MinerName {
            get => NTMinerRoot.Current.MinerProfile.MinerName;
            set {
                if (NTMinerRoot.Current.MinerProfile.MinerName != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(MinerName), value);
                    OnPropertyChanged(nameof(MinerName));
                }
            }
        }

        public bool IsAutoThisPCName {
            get {
                return NTMinerRoot.Current.MinerProfile.IsAutoThisPCName;
            }
            set {
                if (NTMinerRoot.Current.MinerProfile.IsAutoThisPCName != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(IsAutoThisPCName), value);
                    OnPropertyChanged(nameof(IsAutoThisPCName));
                    if (value) {
                        OnPropertyChanged(nameof(MinerName));
                    }
                }
            }
        }

        public bool IsShowInTaskbar {
            get => NTMinerRoot.Current.MinerProfile.IsShowInTaskbar;
            set {
                if (NTMinerRoot.Current.MinerProfile.IsShowInTaskbar != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(IsShowInTaskbar), value);
                    OnPropertyChanged(nameof(IsShowInTaskbar));
                }
            }
        }

        public string ArgsAssembly {
            get {
                return NTMinerRoot.Current.BuildAssembleArgs();
            }
        }

        public bool IsAutoBoot {
            get => NTMinerRoot.Current.MinerProfile.IsAutoBoot;
            set {
                if (NTMinerRoot.Current.MinerProfile.IsAutoBoot != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(IsAutoBoot), value);
                    OnPropertyChanged(nameof(IsAutoBoot));
                }
            }
        }

        public string IsAutoBootText {
            get {
                if (IsAutoBoot) {
                    return "是";
                }
                return "否";
            }
        }

        public bool IsNoShareRestartKernel {
            get => NTMinerRoot.Current.MinerProfile.IsNoShareRestartKernel;
            set {
                if (NTMinerRoot.Current.MinerProfile.IsNoShareRestartKernel != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(IsNoShareRestartKernel), value);
                    OnPropertyChanged(nameof(IsNoShareRestartKernel));
                }
            }
        }

        public int NoShareRestartKernelMinutes {
            get => NTMinerRoot.Current.MinerProfile.NoShareRestartKernelMinutes;
            set {
                if (NTMinerRoot.Current.MinerProfile.NoShareRestartKernelMinutes != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(NoShareRestartKernelMinutes), value);
                    OnPropertyChanged(nameof(NoShareRestartKernelMinutes));
                }
            }
        }

        public bool IsPeriodicRestartKernel {
            get => NTMinerRoot.Current.MinerProfile.IsPeriodicRestartKernel;
            set {
                if (NTMinerRoot.Current.MinerProfile.IsPeriodicRestartKernel != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(IsPeriodicRestartKernel), value);
                    OnPropertyChanged(nameof(IsPeriodicRestartKernel));
                }
            }
        }

        public int PeriodicRestartKernelHours {
            get => NTMinerRoot.Current.MinerProfile.PeriodicRestartKernelHours;
            set {
                if (NTMinerRoot.Current.MinerProfile.PeriodicRestartKernelHours != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(PeriodicRestartKernelHours), value);
                    OnPropertyChanged(nameof(PeriodicRestartKernelHours));
                }
            }
        }

        public bool IsPeriodicRestartComputer {
            get => NTMinerRoot.Current.MinerProfile.IsPeriodicRestartComputer;
            set {
                if (NTMinerRoot.Current.MinerProfile.IsPeriodicRestartComputer != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(IsPeriodicRestartComputer), value);
                    OnPropertyChanged(nameof(IsPeriodicRestartComputer));
                }
            }
        }

        public int PeriodicRestartComputerHours {
            get => NTMinerRoot.Current.MinerProfile.PeriodicRestartComputerHours;
            set {
                if (NTMinerRoot.Current.MinerProfile.PeriodicRestartComputerHours != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(PeriodicRestartComputerHours), value);
                    OnPropertyChanged(nameof(PeriodicRestartComputerHours));
                }
            }
        }

        public bool IsAutoStart {
            get => NTMinerRoot.Current.MinerProfile.IsAutoStart;
            set {
                if (NTMinerRoot.Current.MinerProfile.IsAutoStart != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(IsAutoStart), value);
                    OnPropertyChanged(nameof(IsAutoStart));
                }
            }
        }

        public bool IsAutoRestartKernel {
            get => NTMinerRoot.Current.MinerProfile.IsAutoRestartKernel;
            set {
                if (NTMinerRoot.Current.MinerProfile.IsAutoRestartKernel != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(IsAutoRestartKernel), value);
                    OnPropertyChanged(nameof(IsAutoRestartKernel));
                }
            }
        }

        public bool IsShowCommandLine {
            get { return NTMinerRoot.Current.MinerProfile.IsShowCommandLine; }
            set {
                if (NTMinerRoot.Current.MinerProfile.IsShowCommandLine != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(IsShowCommandLine), value);
                    OnPropertyChanged(nameof(IsShowCommandLine));
                }
            }
        }

        public Guid CoinId {
            get => NTMinerRoot.Current.MinerProfile.CoinId;
            set {
                if (NTMinerRoot.Current.MinerProfile.CoinId != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(CoinId), value);
                    OnPropertyChanged(nameof(CoinId));
                }
            }
        }

        public CoinViewModel CoinVm {
            get {
                CoinViewModel coinVm;
                if (!CoinViewModels.Current.TryGetCoinVm(this.CoinId, out coinVm)) {
                    coinVm = CoinViewModels.Current.AllCoins.FirstOrDefault();
                    if (coinVm != null) {
                        CoinId = coinVm.Id;
                    }
                }
                if (coinVm != null && !coinVm.IsCurrentCoin) {
                    foreach (var item in CoinViewModels.Current.AllCoins) {
                        item.IsCurrentCoin = false;
                    }
                    coinVm.IsCurrentCoin = true;
                }
                return coinVm;
            }
            set {
                if (value != null && !string.IsNullOrEmpty(value.Code)) {
                    this.CoinId = value.Id;
                    OnPropertyChanged(nameof(CoinVm));
                    VirtualRoot.Execute(new RefreshArgsAssemblyCommand());
                }
            }
        }

        public bool IsWorker {
            get {
                return CommandLineArgs.WorkId != Guid.Empty;
            }
        }
    }
}
