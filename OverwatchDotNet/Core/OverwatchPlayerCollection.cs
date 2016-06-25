﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace OverwatchAPI
{
    public delegate void PlayerCollectionStatsUpdated(object sender, EventArgs e);

    class OverwatchPlayerCollection : IEnumerable<OverwatchPlayer>
    {
        private List<OverwatchPlayer> OverwatchPlayers;

        public OverwatchPlayerCollection()
        {
            OverwatchPlayers = new List<OverwatchPlayer>();
        }

        #region AutoUpdate 
        Timer updateIntervalTimer;
        /// <summary>
        /// This event is called whenever the player's stats have finished being auto-updated.
        /// </summary>
        public event PlayerCollectionStatsUpdated StatsUpdated;

        /// <summary>
        /// Start an auto-update timer.
        /// </summary>
        /// <param name="updateInterval">A timespan indicating the amount of time between update cycles.</param>
        public void StartCollectionAutoUpdate(TimeSpan updateInterval)
        {
            updateIntervalTimer = new Timer(updateInterval.TotalSeconds * 1000);
            updateIntervalTimer.Elapsed += UpdateIntervalTimer_Elapsed;
            updateIntervalTimer.Start();
        }

        public void StopCollectionAutoUpdate()
        {
            updateIntervalTimer?.Stop();
            updateIntervalTimer = null;
        }

        private async void UpdateIntervalTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach(var player in OverwatchPlayers)           
                if(player.Region != Region.None)
                    await player.UpdateStats();           
            StatsUpdated?.Invoke(this, e);
        }
        #endregion

        #region List Implementations
        /// <summary>
        /// Adds an overwatch player to the collection (if one with the same battletag does not already exist).
        /// </summary>
        /// <param name="player">An instance of OverwatchPlayer to add.</param>
        /// <returns>True: Player added | False: Player already exists in the collection</returns>
        public bool Add(OverwatchPlayer player)
        {
            var listContains = OverwatchPlayers.Any(x => x.Battletag.Equals(player.Battletag, StringComparison.OrdinalIgnoreCase));
            if (!listContains)
                OverwatchPlayers.Add(player);
            return !listContains;
        }

        /// <summary>
        /// Remove an OverwatchPlayer from the collection.
        /// </summary>
        /// <param name="player">The OverwatchPlayer to remove.</param>
        /// <returns>True: Player removed | False: No player removed</returns>
        public bool Remove(OverwatchPlayer player)
        {
            return OverwatchPlayers.Remove(player);
        }

        /// <summary>
        /// Remove a player with a given battletag.
        /// </summary>
        /// <param name="battletag">The battletag of the player to remove.</param>
        /// <returns>True: Player removed | False: No player removed</returns>
        public bool Remove(string battletag)
        {
            return OverwatchPlayers.RemoveAll(x => x.Battletag.Equals(battletag, StringComparison.OrdinalIgnoreCase)) > 0;
        }

        public OverwatchPlayer this[int index]
        {
            get { return OverwatchPlayers[index]; }
            set { OverwatchPlayers.Insert(index, value); }
        }

        public IEnumerator<OverwatchPlayer> GetEnumerator()
        {
            return OverwatchPlayers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
