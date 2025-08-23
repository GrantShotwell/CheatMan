using Assets.Scripts.Game.Cheats;
using Game.Cheats;
using UnityEngine;
using Zenject;

public class RootInstaller : MonoInstaller {

	[SerializeField] private PlayerController _playerController;
	[SerializeField] private CheatPopupController _cheatPopupController;
	[SerializeField] private CheatDisplayController _cheatDisplayController;

	public override void InstallBindings() {
		InstallCheats();
	}

	private void InstallCheats() {
		Container.BindInterfacesAndSelfTo<PlayerController>()
			.FromInstance(_playerController)
			.AsSingle();
		Container.BindInterfacesAndSelfTo<CheatManager>()
			.AsSingle();
		Container.BindInterfacesAndSelfTo<CheatPopupController>()
			.FromInstance(_cheatPopupController)
			.AsSingle();
		Container.BindInterfacesAndSelfTo<CheatDisplayController>()
			.FromInstance(_cheatDisplayController)
			.AsSingle();
		Container.BindInterfacesAndSelfTo<DashCheat>().AsSingle();
		Container.BindInterfacesAndSelfTo<SuperSpeedCheat>().AsSingle();
		Container.BindInterfacesAndSelfTo<UnlimitedJumpCheat>().AsSingle();
		Container.BindInterfacesAndSelfTo<WallJumpCheat>().AsSingle();
		Container.BindInterfacesAndSelfTo<ZeroGravityCheat>().AsSingle();
	}

}
