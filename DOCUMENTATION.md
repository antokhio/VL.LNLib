# LNLib Documentation

## Table of Contents

- [NURBS Curve](#nurbs-curve)
  - [1. Creation & Fitting](#1-creation--fitting)
  - [2. Querying & Analysis](#2-querying--analysis)
  - [3. Modification & Transformation](#3-modification--transformation)
  - [4. Topology & Structure](#4-topology--structure)

---

## NURBS Curve

The `LNLibNurbsCurve` module provides a comprehensive set of functions for creating, analyzing, and modifying Non-Uniform Rational B-Splines (NURBS) curves.

### 1. Creation & Fitting
Methods for creating new NURBS curves from geometric primitives, interpolation, or approximation.

| Method | Description |
| :--- | :--- |
| **`CreateLine`** | Creates a linear NURBS curve between two points. |
| **`CreateArc`** | Creates a NURBS circular arc. |
| **`CreateOneConicArc`** | Creates a single conic arc segment (Bezier). |
| **`CreateOpenConic`** | Creates an open conic curve. |
| **`CreateCubicHermite`** | Creates a cubic Hermite spline from points and tangents. |
| **`GlobalInterpolation`** | Creates a curve that interpolates through a set of points. |
| **`GlobalInterpolationWthTangents`** | Interpolates points with specified tangent constraints. |
| **`CubicLocalInterpolation`** | Creates a cubic curve using local interpolation. |
| **`LeastSquaresApproximation`** | Approximates a set of points with a curve of a specified degree and control point count. |
| **`WeightedConstrainedLeastSquares`** | A more advanced approximation allowing point weights and tangent constraints. |
| **`GlobalApproximationByErrorBound`** | Approximates points ensuring the error stays within a specified bound. |
| **`FitWithConic`** | Fits a conic segment to a set of points within an error tolerance. |
| **`FitWithCubic`** | Fits a cubic segment to a set of points within an error tolerance. |

### 2. Querying & Analysis
Methods for evaluating point data, derivatives, and properties of an existing curve.

| Method | Description |
| :--- | :--- |
| **`GetPointOnCurve`** | Evaluates the curve at a specific parameter `t`. |
| **`GetPointOnCurveByCornerCut`** | Alternative evaluation method (often more stable for certain algorithms). |
| **`GetParamOnCurve`** | Finds the parameter `t` for a given 3D point on the curve (point inversion). |
| **`ComputeRationalCurveDerivatives`** | Computes derivatives up to a specific order at parameter `t`. |
| **`CanComputeDerivative`** | Checks if derivatives can be computed at `t`. |
| **`Curvature`** | Calculates the curvature at parameter `t`. |
| **`Torsion`** | Calculates the torsion at parameter `t`. |
| **`Normal`** | Computes the normal vector at parameter `t`. |
| **`ProjectNormal`** | Projects normals for the curve. |
| **`ApproximateLength`** | Calculates the total arc length of the curve. |
| **`GetParamByLength`** | Finds the parameter `t` corresponding to a specific arc length from the start. |
| **`GetParamsByEqualLength`** | Returns parameters that divide the curve into segments of equal length. |
| **`Tessellate`** | Generates a polyline approximation of the curve. |
| **`EquallyTessellate`** | Generates tessellation points with equal spacing. |
| **`IsClosed`** | Checks if the curve is closed (start point equals end point). |
| **`IsClamp`** | Checks if the knot vector is clamped (curve touches start/end control points). |
| **`IsPeriodic`** | Checks if the curve is periodic (smoothly closed). |
| **`IsLinear`** | Checks if the curve is a straight line. |
| **`IsArc`** | Checks if the curve is a circular arc. |
| **`ComputerRemoveKnotErrorBound`** | Calculates the error that would result from removing a specific knot. |

### 3. Modification & Transformation
Methods that alter the shape, structure, or parameterization of a curve.

| Method | Description |
| :--- | :--- |
| **`CreateTransformed`** | Applies a 4x4 matrix transformation to the curve. |
| **`Reparametrize`** | Changes the domain of the curve to `[min, max]`. |
| **`ReparametrizeLinearRational`** | Applies a linear rational reparameterization function. |
| **`Reverse`** | Reverses the direction of the curve. |
| **`Offset`** | Creates an offset curve at a specific distance. |
| **`ControlPointReposition`** | Modifies the curve by moving a specific control point. |
| **`WeightModification`** | Modifies the weight of a specific control point. |
| **`NeighborWeightsModification`** | Modifies weights of neighboring control points. |
| **`Warping`** | Applies a warping deformation to the curve. |
| **`Flattening`** | Flattens the curve onto a plane or line. |
| **`Bending`** | Bends the curve around a center/axis. |
| **`ConstraintBasedModification`** | Deforms the curve while respecting geometric constraints (like fixing certain points or derivatives). |

### 4. Topology & Structure
Methods for managing knots, degrees, and segmentation.

| Method | Description |
| :--- | :--- |
| **`InsertKnot`** | Inserts a knot value multiple times without changing the shape. |
| **`RefineKnotVector`** | Inserts multiple knots at once. |
| **`RemoveKnot`** | Removes a knot, potentially changing the shape (approximated). |
| **`RemoveExcessiveKnots`** | Cleans up unnecessary knots. |
| **`RemoveKnotsByGivenBound`** | Removes knots while keeping shape deviation within a tolerance. |
| **`ElevateDegree`** | Increases the degree of the curve without changing shape. |
| **`ReduceDegree`** | Decreases the degree of the curve (approximation). |
| **`ToClampCurve`** | Converts a curve to a clamped knot vector format. |
| **`ToUnclampCurve`** | Converts a curve to an unclamped format. |
| **`DecomposeToBeziers`** | Breaks the NURBS curve into a sequence of Bezier curves. |
| **`SplitAt`** | Splits the curve into two separate curves at parameter `t`. |
| **`Segment`** | Extracts a sub-curve between two parameters. |
| **`Merge`** | Joins two curves into one. |
| **`SplitArc`** | Utility to calculate split points for arc geometry. |