# Grid System

---

### Grid Coordinates  *(PiecesOfWar.Gameplay.Core.Board)*

##### Usage

`// Create coordinates`
`var position = new GridCoordinates(int x, int y);`

`var anotherPosition = new GridCoordinates(int x, int y);`

`// Compare`

`bool areEqual = position == anotherPosition; // Returns true if positions are same`

`position.Equals(anotherPosition) // Returns true if positions are same`



`// Transform`

`position.ToString(); // Returns "(X, Y)" as string`

`position.GetHashCode(); // Generate a hash code (useful for dictionaries/sets)`



### Unit (PiecesOfWar.Gameplay.Core.Entity)


