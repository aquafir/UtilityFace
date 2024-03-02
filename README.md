## Todo

* Fix `ScreenAdjust` for radar


## HUDs

`InterfaceController` starts and controls a number of replacement UIs.



### Chat



### Inventory



### Nav



### Network



### Property Editor



### Radar

### 



## Components

An `IComp` (avoiding MS stuff) is a building blocks for HUDs.  All have:

* `_id` automatically generated using a static field
* Styling options handled internally 
* `Check` which renders the component and returns true if it was interacted with
  * `DrawBody`, called inside of `Check` which draws elements specific to the component
  * `Changed` is set to false at the start and set to true on interaction
* Different data needs handled by the state of the subclass, used when they've been interacted with in a style similar to ImGui
* `Init`/`Dispose` 





### Picker

A `IPicker<T>` draws the elements needed to select something, such as value(s) from an Enum

* Has a `T Selection` for the last element interacted with
* `ICollectionPicker<T>` adds 
  * `T[] Choices` and loops through them in `DrawBody` 
  * `DrawItem(T item, int index)` renders each item
* `IPagedPicker<T>` adds
  * `DrawPageControls` to the start of `DrawBody` 
  * Number of items `PerPage` and the `CurrentPage` are used to display only part of the choices
* `TexturedPicker` 



### Filter

A `IFilter<T>` draws some filtering criteria

* It has a predicate for `T` used for
  * `IEnumerable<T> GetFiltered(IEnumerable<T> input)`
  * `bool IsFiltered(T item)`



An `IOptionalFilter<T>` 

* Adds in a `DrawToggle` which controls the `Active` state of the filter
* That may be used to skip rendering the rest of the filter



Types of filters:

* `RegexFilter` filters based on text for a provided object and a `Func<T, string>` method
* Todo?
  * `ValueComparisonFilter` has a `CompareType` combo and a `double` input that is compares to a `Func<T, double?>` method
  * `BoolFilter` for a list of boolean values

### Modal

A `Modal` is a centered popup for complex inputs

* Keeps track of whether it is `_open` or the negation of that, `Finished`
  * Not drawn if the modal is closed
  * Returns true when the modal is closed
  * `Changed` indicates whether the component was interacted with
* Name kept unique using `_id`
*  `MinSize/MaxSize` used to set constraints
* `Open`/`Close` for related behavior
* Optionally renders as a popup with `IsPopup`
* `PickerModal<T>` displays a Picker and closes if an item is selected.
* *May use a manager/static default version for modals that will only exist one place at a time so a local copy isn't needed*





## Helpers

#### Enum Map

*Map (Prop, value) to Enum type*



* `PropId.TryGetEnum(PropType value)` maps values of a property to their enum, if there is one
* Used to find names / valid values

#### Descriptions

* `wo.Describe` returns a description of a given WorldObject by routing it to the relevant collections of descriptions

  * List of PropType - key?
  * Ignored if empty string / null

* `wo.Describe(PropId key, bool formatted)` exist for each type of property you may want to know about

* A `ComputedProperty` also exists for values that don't directly map to a property value

  * Min damage from variance
  * Calculated resistance of armor
  * etc.

* Dictionaries of [format strings](https://learn.microsoft.com/en-us/dotnet/api/system.string.format?view=net-8.0) exist for each type of property, and if `format=true` that value will be returned instead

  * Possibly make use of `params` for props with more than one input

* `Prop.Friendly(value)` will return a friendly name for a property

  * Based on ACE's  `GetValueEnumName` in `Property<Type>Extensions`
  * Enum values->names
  * Percent conversions like on attack skill
  * etc.

* `Prop.Label()` returns a label for a property

  * Returns a descriptive label for the property like "Ammunition" for `IntId.AmmoType` 

  * Defaults to the name of the property (or number, if not defined in the enum)

    

##### Example Usage

* `wo.Describe(game.World.Selected)` 
  * 

* `wo.Describe(IntId.AmmoType)` 
  * Starts by finding the value:
    `wo.TryGet(IntId.AmmoType)`
    * Returns `null` if missing
  * Then finds the friendly name:
    `wo.Friendly(IntId.AmmoType, value)`  
    * Finds the value on the WorldObject and returns "" if missing
    * If available it calls `Friendly` on the prop with the found value:
      * `IntId.AmmoType.Friendly(4)` ->"Dart"
      * If missing returns `null`?
  * Format is found
    * "Uses {0}s as ammunition." -> 
    * "Uses Darts as ammunition"
  * Format not found
    * "Darts"









## Misc

### Drag/Drop

The `SetDragDropPayload` function allows you to attach data  to the drag and drop operation, and it requires specifying the payload  type, data pointer, and data size.



Operations on pointers are:

* Unary [`&` (address-of)](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/pointer-related-operators#address-of-operator-) operator: to get the address of a variable
* Unary [`*` (pointer indirection)](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/pointer-related-operators#pointer-indirection-operator-) operator: to obtain the variable pointed by a pointer
* The [`->` (member access)](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/pointer-related-operators#pointer-member-access-operator--) and [`[\]` (element access)](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/pointer-related-operators#pointer-element-access-operator-) operators
* Arithmetic operators [`+`, `-`, `++`, and `--`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/pointer-related-operators#pointer-arithmetic-operators)
* Comparison operators [`==`, `!=`, `<`, `>`, `<=`, and `>=`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/pointer-related-operators#pointer-comparison-operators)



Pointer declarations look like:

```
type* identifier;
void* identifier; //allowed but not recommended
```





Reference:

* https://gitlab.com/utilitybelt/utilitybelt.service/-/blob/master/Views/Inspector/Inspector.cs?ref_type=heads#L357
* https://gitlab.com/utilitybelt/utilitybelt.service/-/blob/master/Views/Inspector/MethodInspector.cs?ref_type=heads#L176
* [Blittable types](https://learn.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types)
* [ImGui manager](https://github.com/JaThePlayer/Rysy/blob/3e2c675e43c0a1bfcf88bbae71963e3c2a4f167a/Rysy/Gui/ImGuiManager.cs#L482)
* [Dalamud lib helpers](https://github.com/NightmareXIV/ECommons/blob/aa5305f34b24a7c6955c864675244b8ff5fddfcb/ECommons/ImGuiMethods/ImGuiDragDrop.cs)
