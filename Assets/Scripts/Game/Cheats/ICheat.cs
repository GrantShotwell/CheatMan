namespace Game.Cheats {
	public interface ICheat {
		
		void EnableCheat();

		void DisableCheat();

	}
	public interface ICheat<T> : ICheat where T : ICheatable {

		void OnRegistered(T cheatable);

		void OnUnregistered(T cheatable);

	}
}
