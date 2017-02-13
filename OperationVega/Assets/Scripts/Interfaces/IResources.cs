
namespace Assets.Scripts.Interfaces
{
	/// <summary>
	/// The Resources interface.
	/// </summary>
	public interface IResources
	{
		/// <summary>
		/// Gets or sets the count.
		/// </summary>
		int Count { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether renewable.
		/// </summary>
		bool Renewable { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether taint.
		/// </summary>
		bool Taint { get; set; }

		/// <summary>
		/// The refresh.
		/// </summary>
		void Refresh();

		/// <summary>
		/// The reset.
		/// </summary>
		void Reset();
	}
}