using Assets.Scripts.Game.Cheats;
using Game.Cheats;
using UnityEngine;
using Zenject;

public class RootInstaller : MonoInstaller {

	[SerializeField] private CheatPopupController _cheatPopupController;

	public override void InstallBindings() {
		InstallCheats();
	}

	private void InstallCheats() {
		Container.BindInterfacesAndSelfTo<CheatManager>()
			.AsSingle();
		Container.BindInterfacesAndSelfTo<CheatPopupController>()
			.FromInstance(_cheatPopupController)
			.AsSingle();
		Container.BindInterfacesAndSelfTo<DashCheat>().AsSingle();
		Container.BindInterfacesAndSelfTo<SuperSpeedCheat>().AsSingle();
		Container.BindInterfacesAndSelfTo<UnlimitedJumpCheat>().AsSingle();
		Container.BindInterfacesAndSelfTo<WallJumpCheat>().AsSingle();
		Container.BindInterfacesAndSelfTo<ZeroGravityCheat>().AsSingle();
	}

}
