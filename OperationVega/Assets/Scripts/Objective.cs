
namespace Assets.Scripts
{
    using System;

    using Managers;

    /// <summary>
    /// The objective type.
    /// </summary>
    public enum ObjectiveType
    {
        /// <summary>
        /// The main type.
        /// </summary>
        Main,

        /// <summary>
        /// The kill type.
        /// </summary>
        Kill,

        /// <summary>
        /// The craft type.
        /// </summary>
        Craft
    }

    /// <summary>
    /// The objective.
    /// </summary>
    [Serializable]
    public class Objective
    {
        /// <summary>
        /// The description.
        /// </summary>
        private readonly string description;

        /// <summary>
        /// The type.
        /// </summary>
        private ObjectiveType type;

        /// <summary>
        /// The is completed.
        /// </summary>
        private bool iscompleted;

        /// <summary>
        /// The current value.
        /// </summary>
        private int currentvalue;

        /// <summary>
        /// The max value.
        /// </summary>
        private int maxvalue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Objective"/> class.
        /// <para></para>
        /// <remarks><paramref name="description"></paramref> -The description of the objective.</remarks>
        /// <para></para>
        /// <remarks><paramref name = "type" ></paramref > -The type of the objective.</remarks>
        /// <para></para>
        /// <remarks><paramref name="completed"></paramref> -The boolean whether the objective is completed or not.</remarks>
        /// <para></para>
        /// <remarks><paramref name="startingmin"></paramref> -The integer to start the current objectives count with.</remarks>
        /// <para></para>
        /// <remarks><paramref name="startingmax"></paramref> -The integer to start the current objectives max count with.</remarks>
        /// </summary>
        public Objective(string description, ObjectiveType type, bool completed, int startingmin, int startingmax)
        {
            this.description = description;
            this.type = type;
            this.iscompleted = completed;
            this.currentvalue = startingmin;
            this.maxvalue = startingmax;
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }
        }

        /// <summary>
        /// Gets the type of the objective.
        /// </summary>
        public ObjectiveType Type
        {
            get
            {
                return this.type;
            }

            private set
            {
                this.type = value;
            }

        }

        /// <summary>
        /// Gets or sets a value indicating whether the objective is completed or not.
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                return this.iscompleted;
            }

            set
            {
                this.iscompleted = value;

                if (this.iscompleted)
                {
                    ObjectiveManager.Instance.ObjectiveQueue.Enqueue(this);
                    EventManager.Publish("UpdateObjective");
                }
            }
        }

        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        public int Currentvalue
        {
            get
            {
                return this.currentvalue;
            }

            set
            {
                this.currentvalue = value;

                if (this.currentvalue == this.maxvalue)
                {
                    this.IsCompleted = true;
                }
                else if (this.currentvalue > this.maxvalue)
                {
                    this.currentvalue = this.maxvalue;
                }
            }
        }

        /// <summary>
        /// Gets or sets the max value.
        /// </summary>
        public int Maxvalue
        {
            get
            {
                return this.maxvalue;
            }

            set
            {
                if (this.type != ObjectiveType.Main)
                {
                    this.maxvalue = value;
                }
            }
        }

        /// <summary>
        /// The get objective info function.
        /// Returns the info of this instance of an objective.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetObjectiveInfo()
        {
            if (this.iscompleted && this.type == ObjectiveType.Main)
            {
                this.currentvalue = 1;
            }

            return this.Description + this.Currentvalue + "/" + this.Maxvalue;
        }
    }
}
