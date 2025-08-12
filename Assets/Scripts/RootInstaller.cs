using Assets.Scripts.Game.Cheats;
using Game;
using Game.Cheats;
using UnityEngine;
using Zenject;

public class RootInstaller : MonoInstaller {

	[SerializeField] private GameObject _exampleEnemyPrefab;

	public override void InstallBindings() {
		InstallCheats();
		Container.BindFactory<ExampleEnemy, ExampleEnemy.Factory>()
			.FromComponentInNewPrefab(_exampleEnemyPrefab)
			.AsSingle();
	}

	private void InstallCheats() {
		Container.BindInterfacesAndSelfTo<CheatManager>()
			.AsSingle();
		Container.BindInterfacesAndSelfTo<EnemyHealthCheat>()
			.AsSingle();
	}

}
