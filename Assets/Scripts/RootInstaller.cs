using Assets.Scripts.Game.Cheats;
using Game.Cheats;
using Zenject;

public class RootInstaller : MonoInstaller {

	public override void InstallBindings() {
		InstallCheats();
	}

	private void InstallCheats() {
		Container.BindInterfacesAndSelfTo<CheatManager>()
			.AsSingle();
		Container.BindInterfacesAndSelfTo<EnemyHealthCheat>()
			.AsSingle();
	}

}
