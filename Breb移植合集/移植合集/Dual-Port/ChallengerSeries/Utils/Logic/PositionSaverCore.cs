using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Security.Permissions;
using System.Threading;

using LeagueSharp;
using LeagueSharp.SDK;

using SharpDX;
using EloBuddy;

namespace Challenger_Series.Utils.Logic
{

    /// <summary>
    /// Original Code Credit: AiM
    /// #TODO: this sucks, improve.
    /// </summary>
    internal class PositionSaverCore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PositionSaverCore"/> class. 
        /// </summary>

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public PositionSaverCore()
        {
            #region Initialize
            DoChecks();
            #endregion
        }

        #region Fields

        /// <summary>
        /// List of the Saved Positions loaded in memory.
        /// </summary>
        public List<Vector3> Positions = new List<Vector3>();

        /// <summary>
        /// The root of the folder.
        /// </summary>
        private static string PositionSaverDirectory = FileOperations.AppDataDirectory + @"\Challenger Series\"+ObjectManager.Player.CharData.BaseSkinName+@"\Position Saver\";

        /// <summary>
        /// File Location for X
        /// </summary>
        private static string xFile = PositionSaverDirectory + (int)Game.MapId + @"\" + "xFile" + ".txt";

        /// <summary>
        /// File Location for Y
        /// </summary>
        private static string yFile = PositionSaverDirectory + (int)Game.MapId + @"\" + "yFile" + ".txt";

        /// <summary>
        /// Array of X Integer
        /// </summary>
        private int[] xInt;

        /// <summary>
        /// Array of Y Integer
        /// </summary>
        private int[] yInt;

        #endregion

        #region Methods

        /// <summary>
        /// Checks for missing files, Converts the values to integer, then adds them into a Vector3 List
        /// </summary>
        private void DoChecks()
        {
            #region Check Missing Files

            if (!Directory.Exists(PositionSaverDirectory))
            {
                Directory.CreateDirectory(PositionSaverDirectory);
                for (var i = 8; i < 18; i++ )
                {
                    Directory.CreateDirectory(PositionSaverDirectory + i);
                }
                CreateFile();
            }
            if (!Directory.Exists(PositionSaverDirectory + (int)Game.MapId + @"\"))
            {
                for (var i = 8; i < 18; i++)
                {
                    Directory.CreateDirectory(PositionSaverDirectory + i);
                }
                CreateFile();
            }
            else if (!File.Exists(xFile) || !File.Exists(yFile))
            {
                CreateFile();
            }
            else if (File.Exists(xFile) && File.Exists(yFile))
            {
                ConvertToInt();
            }

            #endregion
        }

        /// <summary>
        /// Creates Files that are missing
        /// </summary>
        private void CreateFile()
        {
                File.Create(xFile).Dispose();
                File.Create(yFile).Dispose();

            DoChecks();
        }

        /// <summary>
        /// Populates the <see cref="Positions"/> list with saved values.
        /// </summary>
        public void PopulatePositionList()
        {
            #region Get Location
            for (var i = 0; i < xInt.Length; i++)
            {
                Positions.Add(new Vector2(xInt[i], yInt[i]).ToVector3());
            }

            #endregion
        }

        public void SavePosition(Vector3 position)
        {
            using (var writer = new StreamWriter(xFile, true))
            {
                writer.WriteLine(Math.Round(position.X));
            }
            using (var writer = new StreamWriter(yFile, true))
            {
                writer.WriteLine(Math.Round(position.Y));
                writer.Dispose();
            }
        }
        public void RemovePosition(Vector3 position)
        {
            try
            {
                var elementInList = Positions.FirstOrDefault(p => p.Distance(position) < 100);
                Positions.Remove(elementInList);

                File.WriteAllText(xFile, string.Empty);
                File.WriteAllText(yFile, string.Empty);

                foreach (var pos in Positions)
                {
                    SavePosition(pos);
                }
            }
            catch (Exception)
            {

            }
        }

        public void PurgeAllPositions()
        {
            Positions.Clear();
            File.WriteAllText(xFile, string.Empty);
            File.WriteAllText(yFile, string.Empty);
        }

        /// <summary>
        /// Converts String to Integer
        /// </summary>
        private void ConvertToInt()
        {
            #region Convert to Int
            var xLines = File.ReadAllLines(xFile);
            var yLines = File.ReadAllLines(yFile);
            xInt = new int[xLines.Length];
            yInt = new int[yLines.Length];

            for (var i = 0; i < xLines.Length; i++)
            {
                xInt[i] = Convert.ToInt32(xLines[i]);
            }

            for (var i = 0; i < xLines.Length; i++)
            {
                yInt[i] = Convert.ToInt32(yLines[i]);
            }

            PopulatePositionList();
            #endregion
        }

        #endregion
    }
}