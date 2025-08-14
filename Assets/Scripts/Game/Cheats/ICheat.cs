namespace Game.Cheats {
	public interface ICheat<T> where T : ICheatable {

		void EnableCheat();

		void DisableCheat();

		void OnRegistered(T cheatable);

		void OnUnregistered(T cheatable);

	}
}
