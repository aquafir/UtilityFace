## HUDs

`InterfaceController` starts and controls a number of replacement UIs.



### Chat



### Inventory



### Nav



### Network



### Property Editor



### Radar

### 



## Pickers / Modals

Help selecting things like icons or spells.



Modals have a static default version and may have other versions made for customization.





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
