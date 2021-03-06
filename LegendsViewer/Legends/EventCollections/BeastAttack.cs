﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LegendsViewer.Controls;

namespace LegendsViewer.Legends
{
    public class BeastAttack : EventCollection
    {
        public int Ordinal { get; set; }
        public Location Coordinates { get; set; }
        public WorldRegion Region { get; set; }
        public UndergroundRegion UndergroundRegion { get; set; }
        public Site Site { get; set; }
        public Entity Defender { get; set; }
        public HistoricalFigure Beast { get; set; }
        public List<HistoricalFigure> Deaths { get { return GetSubEvents().OfType<HFDied>().Select(death => death.HistoricalFigure).ToList(); } set { } }
        public static List<string> Filters;
        public override List<WorldEvent> FilteredEvents
        {
            get { return AllEvents.Where(dwarfEvent => !Filters.Contains(dwarfEvent.Type)).ToList(); }
        }
        public BeastAttack()
            : base()
        {
            Initialize();
        }
        public BeastAttack(List<Property> properties, World world)
            : base(properties, world)
        {
            Initialize();

            foreach (Property property in properties)
                switch (property.Name)
                {
                    case "ordinal": Ordinal = Convert.ToInt32(property.Value); break;
                    case "coords": Coordinates = Formatting.ConvertToLocation(property.Value); break;
                    case "parent_eventcol": ParentCollection = world.GetEventCollection(Convert.ToInt32(property.Value)); break;
                    case "subregion_id": Region = world.GetRegion(Convert.ToInt32(property.Value)); break;
                    case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
                    case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                    case "defending_enid": Defender = world.GetEntity(Convert.ToInt32(property.Value)); break;
                }

            Site.BeastAttacks.Add(this);

            //--------Attacking Beast is calculated after parsing event collections in ParseXML()
            //--------So that it can also look at eventsList from duel sub collections to calculate the Beast

            //-------Fill in some missing event details with details from collection
            //-------Filled in after parsing event collections in ParseXML()
        }

        private void Initialize()
        {
            Ordinal = 1;
            Coordinates = new Location(0, 0);
        }

        public override string ToLink(bool link = true, DwarfObject pov = null)
        {
            string name = "";
            name = "The " + GetOrdinal(Ordinal) + "rampage of ";
            if (Beast != null && pov == Beast) name += Beast.ToLink(false, Beast);
            else if (Beast != null) name += Beast.Name;
            else name += "UNKNOWN BEAST";
            if (pov != Site) name += " in " + Site.ToLink(false);
            if (link)
            {
                string linkedString = "";
                if (pov != this)
                {
                    string title = "Events: " + GetSubEvents().Count;

                    linkedString = "<a href = \"collection#" + this.ID + "\" title=\"" + title + "\"><font color=\"#336600\">" + name + "</font></a>";
                }
                else
                    linkedString = "<font color=\"Blue\">" + name + "</font>";
                return linkedString;
            }
            else
                if (pov == this) return "Rampage of " + Beast.ToLink(false, Beast);
            return name;
        }

        public override string ToString()
        {
            return ToLink(false);
        }

    }
}
