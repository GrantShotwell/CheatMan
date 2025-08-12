using Game;
using Game.Cheats;
using UnityEngine;
using Zenject;

public class RootInstaller : MonoInstaller {

	[SerializeField] private GameObject _exampleEnemyPrefab;

	public override void InstallBindings() {
		Container.BindInterfacesAndSelfTo<CheatManager>()
			.AsSingle();
		Container.BindFactory<ExampleEnemy, ExampleEnemy.Factory>()
			.FromComponentInNewPrefab(_exampleEnemyPrefab)
			.AsSingle();
	}

}
