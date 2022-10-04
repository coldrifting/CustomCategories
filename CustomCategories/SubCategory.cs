using System;
using System.Collections.Generic;
using KSP.UI.Screens;

namespace CustomCategories
{
    internal class SubCategory : IComparable<SubCategory>
    {
        public string Name { get; }
        public string DisplayName { get; set; }
        public int Order { get; set; }
        public RUI.Icons.Selectable.Icon Icon { get; set; }

        public HashSet<string> PartFilter { get; }

        public SubCategory(string name)
        {
            Name = name;
            DisplayName = name;
            Icon = PartCategorizer.Instance.iconLoader.iconDictionary["stockIcon_fallback"];

            // Put new subcategories after stock ones by default
            Order = 20000; 

            PartFilter = new HashSet<string>();

            FixStockCategories();
        }

        public void AddPart(string partName)
        {
            PartFilter.Add(partName);
        }

        public bool Filter(AvailablePart part)
        {
            return PartFilter.Contains(part.name);
        }

        public int CompareTo(SubCategory other)
        {
            int compare = Order.CompareTo(other.Order);

            if (compare != 0)
                return compare;

            return String.CompareOrdinal(Name, other.Name);
        }

        /// <summary>
        /// Adjust stock categories to match their default configuration.
        /// These settings can be overriden by user configuration later.
        /// </summary>
        private void FixStockCategories()
        {
            switch (Name)
            {
                case "Pods":
                    Icon = Icons.GetIcon("stockIcon_pods");
                    Order = 1000;
                    break;

                case "FuelTank":
                    DisplayName = "Fuel Tanks";
                    Icon = Icons.GetIcon("stockIcon_fueltank");
                    Order = 2000;
                    break;

                case "Engine":
                    Icon = Icons.GetIcon("stockIcon_engine");
                    Order = 3000;
                    break;

                case "Control":
                    Icon = Icons.GetIcon("stockIcon_cmdctrl");
                    Order = 4000;
                    break;

                case "Structural":
                    Icon = Icons.GetIcon("stockIcon_structural");
                    Order = 5000;
                    break;

                case "Coupling":
                    Icon = Icons.GetIcon("stockIcon_coupling");
                    Order = 6000;
                    break;

                case "Payload":
                    Icon = Icons.GetIcon("stockIcon_payload");
                    Order = 7000;
                    break;

                case "Aero":
                    DisplayName = "Aerodynamics";
                    Icon = Icons.GetIcon("stockIcon_aerodynamics");
                    Order = 8000;
                    break;

                case "Ground":
                    Icon = Icons.GetIcon("stockIcon_ground");
                    Order = 9000;
                    break;

                case "Thermal":
                    Icon = Icons.GetIcon("stockIcon_thermal");
                    Order = 10000;
                    break;

                case "Electrical":
                    Icon = Icons.GetIcon("stockIcon_electrical");
                    Order = 11000;
                    break;

                case "Communication":
                    Icon = Icons.GetIcon("stockIcon_communication");
                    Order = 12000;
                    break;

                case "Science":
                    Icon = Icons.GetIcon("stockIcon_science");
                    Order = 13000;
                    break;

                case "Cargo":
                    Icon = Icons.GetIcon("stockIcon_cargo");
                    Order = 14000;
                    break;

                case "Robotics":
                    Icon = Icons.GetIcon("serenityIcon_robotics");
                    Order = 15000;
                    break;

                case "Utility":
                    Icon = Icons.GetIcon("stockIcon_utility");
                    Order = 16000;
                    break;

                default:
                    // Do nothing
                    break;
            }
        }
    }
}
