using System;
using UdonSharp;
using VRC.SDK3.Data;
using VRC.SDKBase;

namespace net.puk06.Utils
{
    /// <summary>
    /// This is a collection of commonly used functions in UdonSharp. It allows you to retrieve players within an instance and perform array operations.
    /// </summary>
    public class UdonUtils : UdonSharpBehaviour
    {
        #region VRChat Player Methods
        /// <summary>
        /// Retrieves all players within the instance. Players in the array may be null. Please check using Utilities.IsValid().
        /// </summary>
        /// <returns></returns>
        public static VRCPlayerApi[] GetAllPlayers()
        {
            int playerCount = VRCPlayerApi.GetPlayerCount();
            VRCPlayerApi[] playersArray = new VRCPlayerApi[playerCount];
            return VRCPlayerApi.GetPlayers(playersArray);
        }

        /// <summary>
        /// Returns a DataList containing the usernames of all players in the instance. Invalid players are ignored.
        /// </summary>
        /// <returns></returns>
        public static DataList GetAllPlayerNames()
        {
            VRCPlayerApi[] playersArray = GetAllPlayers();
            DataList playerNamesList = new DataList();

            foreach (VRCPlayerApi player in playersArray)
            {
                if (!Utilities.IsValid(player)) continue;
                playerNamesList.Add(player.displayName);
            }

            return playerNamesList;
        }

        /// <summary>
        /// Retrieves the player by ID. Returns null if the user is invalid.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public static VRCPlayerApi GetPlayerById(int playerId)
        {
            VRCPlayerApi player = VRCPlayerApi.GetPlayerById(playerId);
            if (!Utilities.IsValid(player)) return null;

            return player;
        }

        /// <summary>
        /// You can obtain the LocalPlayer. When acquiring the player, it automatically checks whether the LocalPlayer is an invalid object. If it is invalid, it returns null.
        /// </summary>
        /// <returns></returns>
        public static VRCPlayerApi GetLocalPlayer()
        {
            if (!Utilities.IsValid(Networking.LocalPlayer)) return null;
            return Networking.LocalPlayer;
        }
        #endregion

        #region Array Methods
        /// <summary>
        /// You can check whether the passed value is contained in the passed array. If it exists, the index is returned; otherwise, -1 is returned. You cannot check whether both values are null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ArrayContains<T>(T[] array, T value)
        {
            if (array == null || value == null || array.Length == 0) return -1;

            for (int i = 0; i < array.Length; i++)
            {
                T arrayValue = array[i];
                if (arrayValue == null) continue;
                if (arrayValue.Equals(value)) return i;
            }

            return -1;
        }

        /// <summary>
        /// Automatically parses an array containing comma-separated strings and compares the value at a specific index position. If the value exists at that index, it returns that index; otherwise, it returns -1. For example, for an array like [“value1...value2”, “value1...value2”], it checks the passed value against the index position within the array obtained by parsing the entire array contents (here, 0 corresponds to value1).
        /// </summary>
        /// <param name="dataArray"></param>
        /// <param name="parsedDataArrayIndex"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int DataArrayContains(string[] dataArray, int parsedDataArrayIndex, string value)
        {
            if (dataArray == null || value == null || dataArray.Length == 0) return -1;

            for (int i = 0; i < dataArray.Length; i++)
            {
                string arrayValue = dataArray[i];
                if (arrayValue == null || arrayValue == "") continue;

                string[] parsedStrings = ParseDataString(arrayValue);
                if (parsedDataArrayIndex < 0 || parsedDataArrayIndex >= parsedStrings.Length) continue;

                if (parsedStrings[parsedDataArrayIndex].Equals(value)) return i;
            }

            return -1;
        }

        /// <summary>
        /// Adds the passed value to the passed array. If the passed array is null, returns an empty array. If the passed value is null, returns the original array. If the value is added, returns a new array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[] ArrayAdd<T>(T[] array, T value)
        {
            if (array == null) return new T[0];
            if (value == null) return array;

            var newArray = new T[array.Length + 1];
            array.CopyTo(newArray, 0);
            newArray[array.Length] = value;

            return newArray;
        }
        #endregion

        #region Comma-Separated String Methods
        /// <summary>
        /// This function can parse comma-separated arrays commonly used for synchronization. For example, “value1,,,value2” will return string values: `Array[0]` holds `value1` and `Array[1]` holds `value2`. To convert to numeric values, use `int.Parse()` and access them via `Array[index]`.
        /// </summary>
        /// <param name="dataString"></param>
        /// <returns></returns>
        public static string[] ParseDataString(string dataString)
        {
            return dataString.Split(new string[] { ",,," }, StringSplitOptions.None);
        }
        #endregion

        #region UdonSharp Non-Supported Methods
        /// <summary>
        /// It behaves the same as Math.Clamp. If the given value is less than the minimum value, the minimum value is returned; if the given value is greater than the minimum value, the maximum value is returned.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static byte Clamp(byte value, byte minValue, byte maxValue)
        {
            if (value < minValue) value = minValue;
            else if (value > maxValue) value = maxValue;

            return value;
        }

        /// <summary>
        /// It behaves the same as Math.Clamp. If the given value is less than the minimum value, the minimum value is returned; if the given value is greater than the minimum value, the maximum value is returned.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int Clamp(int value, int minValue, int maxValue)
        {
            if (value < minValue) value = minValue;
            else if (value > maxValue) value = maxValue;

            return value;
        }

        /// <summary>
        /// It behaves the same as Math.Clamp. If the given value is less than the minimum value, the minimum value is returned; if the given value is greater than the minimum value, the maximum value is returned.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static double Clamp(double value, double minValue, double maxValue)
        {
            if (value < minValue) value = minValue;
            else if (value > maxValue) value = maxValue;

            return value;
        }

        /// <summary>
        /// It behaves the same as Math.Clamp. If the given value is less than the minimum value, the minimum value is returned; if the given value is greater than the minimum value, the maximum value is returned.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static float Clamp(float value, float minValue, float maxValue)
        {
            if (value < minValue) value = minValue;
            else if (value > maxValue) value = maxValue;

            return value;
        }
        #endregion

        #region String Methods
        /// <summary>
        /// Colors the given string using the specified color code. The # symbol is optional.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="colorCode"></param>
        /// <returns></returns>
        public static string ColorizeString(string value, string colorCode)
        {
            if (colorCode == null || colorCode == "") return value;

            if (colorCode.Length == 6) return $"<color=#{colorCode}>{value}</color>";
            else if (colorCode.Length == 7) return $"<color={colorCode}>{value}</color>";

            return value;
        }
        #endregion
    }
}
