namespace Game.Cheats {
	public interface ICheat {

		string DisplayName { get; }

		bool Enabled { get; }
		
		void EnableCheat();

		void DisableCheat();

		bool TryRegister(ICheatable cheatable);

		bool TryUnregister(ICheatable cheatable);

	}
	public interface ICheat<T> : ICheat where T : ICheatable {

		void OnRegistered(T cheatable);

		void OnUnregistered(T cheatable);

	}
}
