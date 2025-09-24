# UdonUtils — Common Utility Functions for UdonSharp

This is a collection of commonly used functions in UdonSharp. It allows you to retrieve players within an instance and perform array operations.

# Available Methods

## VRChat Player Methods

### `UdonUtils.GetAllPlayers()`

- **Return**: `VRCPlayerApi?[]`  

Retrieves all players within the instance. Players in the array may be null. Please check using Utilities.IsValid().

### `UdonUtils.GetAllPlayerNames()`

- **Return**: `DataList`
  
  Returns a DataList containing the usernames of all players in the instance. Invalid players are ignored.

### `UdonUtils.GetPlayerById(int playerId)`

- **Return**: `VRCPlayerApi?`
  
  Retrieves the player by ID. Returns null if the user is invalid.

### `UdonUtils.GetLocalPlayer()`

- **Return**: `VRCPlayerApi?`
  
  You can obtain the LocalPlayer. When acquiring the player, it automatically checks whether the LocalPlayer is an invalid object. If it is invalid, it returns null.
  
  

## Array Methods

### `UdonUtils.ArrayContains<T>(T[] array, T value)`

- **Return**: `int`
  
  You can check whether the passed value is contained in the passed array. If it exists, the index is returned; otherwise, -1 is returned. You cannot check whether both values are null.

### `UdonUtils.DataArrayContains(string[] dataArray, int parsedDataArrayIndex, string value)`

- **Return**: `int`

Automatically parses an array containing comma-separated strings and compares the value at a specific index position. If the value exists at that index, it returns that index; otherwise, it returns -1. For example, for an array like [“value1...value2”, “value1...value2”], it checks the passed value against the index position within the array obtained by parsing the entire array contents (here, 0 corresponds to value1).

### `UdonUtils.ArrayAdd<T>(T[] array, T value)`

- **Return**: `T[]`

Adds the passed value to the passed array. If the passed array is null, returns an empty array. If the passed value is null, returns the original array. If the value is added, returns a new array.



## Comma-Separated String Methods

### `UdonUtils.ParseDataString(string dataString)`

- **Return**: `string[]`

This function can parse comma-separated arrays commonly used for synchronization. For example, “value1,,,value2” will return string values: `Array[0]` holds `value1` and `Array[1]` holds `value2`. To convert to numeric values, use `int.Parse()` and access them via `Array[index]`.



## UdonSharp Non-Supported Methods

### `UdonUtils.Clamp(value, minValue, maxValue)`

- **Supported types**: `byte`, `int`, `float`, `double`  

It behaves the same as Math.Clamp. If the given value is less than the minimum value, the minimum value is returned; if the given value is greater than the minimum value, the maximum value is returned.



## String Methods

### `UdonUtils.ColorizeString(string value, string colorCode)`

- **Return**: `string`

Colors the given string using the specified color code. The # symbol is optional.
