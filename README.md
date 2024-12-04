# CSLibreLinkUp for .NET

Obtain Glucose Values and more from LibreLinkUp

## Description

The LluClient class enables you to easily obtain glucose values from LibreLinkUp using .NET

## Usage

```cs
var lluClient = new LluClient("<..LibreLinkUp_Email..>", "<..LibreLinkUpPassword..>");
int status = await lluClient.ConnectAsync();
```

If the status is 0 then you can execute one of the following:

### ReadBasicAsync()

```cs
LluData glucoseData = await lluClient.ReadBasicAsync();
```

This will return an LluData class containing the following:

```cs
public string PatientId { get; private set; }
public double Value { get; private set; }
public int ValueInMgPerDl { get; private set; }
public bool IsHigh { get; private set; }
public bool IsLow { get; private set; }
public Trend TrendArrow { get; private set; }
public DateTime TimeStamp { get; private set; }
```

### ReadAsync()

```cs
dynamic glucoseData = await lluClient.ReadAsync();
```

This will return the full raw contents of current glucose values, returned from the LibreLinkUp api in Json format.

To obtain a value do the following:

```cs
double glucoseValue = Convert.ToDouble(connection.data[0].glucoseMeasurement.Value);
```

### ReadGraphAsync(patientId)

```cs
dynamic graph = await _lluClient.ReadGraphAsync(patientId);
```

This will return the full raw contents of current, and historical glucose values, returned from the LibreLinkUp api in Json format.

You can get the `PatientId` from either the `ReadBasicAsync()` or `ReadAsync()` methods:

##### ReadBasicAsync()

```cs
LluData glucoseData = await lluClient.ReadBasicAsync();
string patientId = glucoseData.PatientId;
```

##### ReadAsync()

```cs
dynamic glucoseData = await lluClient.ReadAsync();
string patientId = Convert.ToString(connections.data[0].patientId);
```

To obtain a value do the following:

```cs
double glucoseValue = Convert.ToDouble(graph.data.connection.glucoseMeasurement.Value);
```

## Contact

Matthew Burrows

Email: [Mattyb2001uk@outlook.com](Mattyb2001uk@outlook.com)
