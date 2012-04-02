using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Math3D;
using GridWorld;
using OpenTK;

namespace Game
{
    public class ControledActor : GameState.BoundableActor
    {
        public ControledActor(GameState state)
            : base(state)
        {

        }

        public double LastUpdateTime = 0;
        public Vector3 LastUpdatePostion = Vector3.Zero;
        public Vector3 LastUpdateRotation = Vector3.Zero;

        protected UInt64 LastHistoryID = 0;

        public class HistoryItem
        {
            public UInt64 HistoryID = UInt64.MaxValue;
            public double InputTime = -1;
            public Vector3 Direction = Vector3.Zero;
            public Vector3 Rotation = Vector3.Zero;

            public Vector3 Position = Vector3.Zero;
            public Vector3 Orientation = Vector3.Zero;

            public static HistoryItem Empty = new HistoryItem();
        }

        public HistoryItem PredictionVector = HistoryItem.Empty;

        public List<HistoryItem> InputHistory = InitalHistory();

        protected static List<HistoryItem> InitalHistory()
        {
            List<HistoryItem> list = new List<HistoryItem>();
            HistoryItem item = new HistoryItem();
            item.InputTime = -1;
            item.HistoryID = 0;
            list.Add(item);
            return list;
        }

        public bool LogUpdates = false;

        public List<string> UpdateLog = new List<string>();

        public void SetKnownState(double time, Vector3 postion, Vector3 rotation)
        {
            LastUpdateTime = time;
            LastUpdatePostion = postion;
            LastUpdateRotation = rotation;

//             List<HistoryItem> toRemove = new List<HistoryItem>();
//             foreach (HistoryItem item in InputHistory)
//             {
//                 if (item.InputTime < time)
//                     toRemove.Add(item);
//             }
// 
//             foreach (HistoryItem item in toRemove)
//                 InputHistory.Remove(item);
        }

        public void AddHistoryItem(HistoryItem item)
        {
            if (InputHistory.Count == 0)
                item.HistoryID = 0;
            else
            {
                LastHistoryID++;
                item.HistoryID = LastHistoryID;
            }

            InputHistory.Add(item);
        }

        public void AddHistoryItem(double time, Vector3 dir, Vector3 rot)
        {
            HistoryItem h = new HistoryItem();
            h.Direction = dir;
            h.Rotation = rot;

            h.InputTime = time;
            AddHistoryItem(h);
        }

        public HistoryItem FindInputJustBeforeTime( double time )
        {
            HistoryItem lastItem = HistoryItem.Empty;
            foreach ( HistoryItem item in InputHistory)
            {
                if (item.InputTime <= time)
                    lastItem = item;
                else
                    break;
            }
            return lastItem;
        }

        public bool ClearHistoryBeforeTime(double time)
        {
            int lastItem = -1;
            for (int i = 0; i < InputHistory.Count; i++)
            {
                if (InputHistory[i].InputTime < time)
                    lastItem = i-1;
                else
                    break;
            }

            if (lastItem == -1)
                return false;
            if (lastItem > 1)
                InputHistory.RemoveRange(0, lastItem);

            return true;
        }

        protected float CylinderRadius = 1.0f;
        protected float CylinderHeight = 0.5f;

        protected BoundingBox bbox = BoundingBox.Empty;

        public override BoundingBox GetBounds()
        {
            Vector3 position = GetLocation().Position;

            if (bbox == BoundingBox.Empty)
                bbox = new BoundingBox(position, position);

            bbox.Max.X = position.X + CylinderRadius;
            bbox.Max.Y = position.Y + CylinderRadius;
            bbox.Max.Z = position.Z + CylinderHeight;

            bbox.Min.X = position.X - CylinderRadius;
            bbox.Min.Y = position.Y - CylinderRadius;
            bbox.Min.Z = position.Z;

            return bbox;
        }

        protected void AddUpdateLog(string text)
        {
            if (!LogUpdates)
                return;
            UpdateLog.Add(text);
        }

        // TODO. keep the first item as the vector to use from last known pos to next update so we always have valid vectors.
        // TODO. check removal code to ensure that when we reset to a known good state we don't dobule update it.

        public override Location GetLocationAtTime(double time)
        {
            AddUpdateLog("Start update for " + time.ToString() + " from " + LastUpdateTime.ToString());

            Location loc = new Location();
            loc.Position = LastUpdatePostion;
            loc.Rotation = LastUpdateRotation;

            AddUpdateLog("Update initial Position " + loc.Position.ToString());
            AddUpdateLog("Update initial Rotation " + loc.Rotation.ToString());

            double lastProjectionTime = LastUpdateTime;
            double delta = 0;

            if (PredictionVector != HistoryItem.Empty)
            {
                // just project that sucker out to where we are now
                delta = time - lastProjectionTime;
                AddUpdateLog("Prediction Vector delta " + delta.ToString());

                AddUpdateLog("Prediction Vector dir " + PredictionVector.Direction.ToString());
                AddUpdateLog("Prediction Vector rot " + PredictionVector.Rotation.ToString());

                loc.Position += PredictionVector.Direction * (float)delta;
                loc.Rotation += PredictionVector.Rotation * (float)delta;
            }
            else
            {
                // use the previous input state to run us up to the time of each state
                if (InputHistory.Count > 1)
                {
                    int lastStep = 0;

                    for (int i = 1; i < InputHistory.Count; i++)
                    {
                        if (InputHistory[i].InputTime > LastUpdateTime) // only use history items that are after our known good time
                        {
                            if (InputHistory[i].InputTime >= time)
                            {
                                AddUpdateLog("Skipping History Step(" + InputHistory[i].HistoryID.ToString() + ") Due to time " + InputHistory[i].InputTime.ToString());
                                break;
                            }
                            lastStep = i;

                            delta = InputHistory[i].InputTime - lastProjectionTime;
                            lastProjectionTime = InputHistory[i].InputTime;

                            AddUpdateLog("Processing History Item " + InputHistory[i].HistoryID.ToString() + " to time " + InputHistory[i].InputTime.ToString() + "(delta " + delta.ToString() + ")");

                            loc.Position += InputHistory[i - 1].Direction * (float)delta;
                            loc.Rotation += InputHistory[i - 1].Rotation * (float)delta;

                            AddUpdateLog("Update Vector dir " + InputHistory[i - 1].Direction.ToString());
                            AddUpdateLog("Update Vector rot " + InputHistory[i - 1].Rotation.ToString());

                            InputHistory[i].Position = new Vector3(loc.Position);
                            InputHistory[i].Orientation = new Vector3(loc.Rotation);

                            AddUpdateLog("Update Location Lin " + loc.Position.ToString());
                            AddUpdateLog("Update Location Rot " + loc.Rotation.ToString());
                        }
                    }

                    delta = time - lastProjectionTime;

                    AddUpdateLog("Processing Last History Step(" + InputHistory[lastStep].HistoryID.ToString() + ") to time " + time.ToString() + " (delta " + delta.ToString() + ")");

                    AddUpdateLog("Update Vector dir " + InputHistory[lastStep].Direction.ToString());
                    AddUpdateLog("Update Vector rot " + InputHistory[lastStep].Rotation.ToString());

                    loc.Position += InputHistory[lastStep].Direction * (float)delta;
                    loc.Rotation += InputHistory[lastStep].Rotation * (float)delta;
                }
                else
                {
                    if (InputHistory.Count > 0)
                    {
                        // we just want to project the one item we have to now just like we would if we had no history
                        delta = time - lastProjectionTime;

                        AddUpdateLog("Processing Only History Step (" + InputHistory[0].HistoryID.ToString() + ") to time " + time.ToString() + " (delta " + delta.ToString() + ")");

                        loc.Position += InputHistory[0].Direction * (float)delta;
                        loc.Rotation += InputHistory[0].Rotation * (float)delta;

                        AddUpdateLog("Update Vector dir " + InputHistory[0].Direction.ToString());
                        AddUpdateLog("Update Vector rot " + InputHistory[0].Rotation.ToString());
                    }
                    else
                    {
                        AddUpdateLog("Empty History!!!!!!");
                    }
                }
            }

            AddUpdateLog("Final Loc dir " + loc.Position.ToString());
            AddUpdateLog("Prediction Loc rot " + loc.Rotation.ToString());
            return loc;
        }

        public override bool Update()
        {
            base.Update();

            CurrentLocation = GetLocationAtTime(State.Now);

            Moved = CurrentLocation.Position != LastUpdatePostion;
            return true;
        }
    }
}
