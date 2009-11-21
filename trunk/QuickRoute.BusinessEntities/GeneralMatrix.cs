using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace QuickRoute.BusinessEntities
{
  #region Internal Maths utility
  internal class Maths
  {
    /// <summary>
    ///  sqrt(a^2 + b^2) without under/overflow.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>

    public static double Hypot(double a, double b)
    {
      double r;
      if (Math.Abs(a) > Math.Abs(b))
      {
        r = b / a;
        r = Math.Abs(a) * Math.Sqrt(1 + r * r);
      }
      else if (b != 0)
      {
        r = a / b;
        r = Math.Abs(b) * Math.Sqrt(1 + r * r);
      }
      else
      {
        r = 0.0;
      }
      return r;
    }
  }
  #endregion   // Internal Maths utility

  /// <summary>.NET GeneralMatrix class.
  /// 
  /// The .NET GeneralMatrix Class provides the fundamental operations of numerical
  /// linear algebra.  Various constructors create Matrices from two dimensional
  /// arrays of double precision floating point numbers.  Various "gets" and
  /// "sets" provide access to submatrices and matrix elements.  Several methods 
  /// implement basic matrix arithmetic, including matrix addition and
  /// multiplication, matrix norms, and element-by-element array operations.
  /// Methods for reading and printing matrices are also included.  All the
  /// operations in this version of the GeneralMatrix Class involve real matrices.
  /// Complex matrices may be handled in a future version.
  /// 
  /// Five fundamental matrix decompositions, which consist of pairs or triples
  /// of matrices, permutation vectors, and the like, produce results in five
  /// decomposition classes.  These decompositions are accessed by the GeneralMatrix
  /// class to compute solutions of simultaneous linear equations, determinants,
  /// inverses and other matrix functions.  The five decompositions are:
  /// <P><UL>
  /// <LI>Cholesky Decomposition of symmetric, positive definite matrices.
  /// <LI>LU Decomposition of rectangular matrices.
  /// <LI>QR Decomposition of rectangular matrices.
  /// <LI>Singular Value Decomposition of rectangular matrices.
  /// <LI>Eigenvalue Decomposition of both symmetric and nonsymmetric square matrices.
  /// </UL>
  /// <DL>
  /// <DT><B>Example of use:</B></DT>
  /// <P>
  /// <DD>Solve a linear system A x = b and compute the residual norm, ||b - A x||.
  /// <P><PRE>
  /// double[][] vals = {{1.,2.,3},{4.,5.,6.},{7.,8.,10.}};
  /// GeneralMatrix A = new GeneralMatrix(vals);
  /// GeneralMatrix b = GeneralMatrix.Random(3,1);
  /// GeneralMatrix x = A.Solve(b);
  /// GeneralMatrix r = A.Multiply(x).Subtract(b);
  /// double rnorm = r.NormInf();
  /// </PRE></DD>
  /// </DL>
  /// </summary>
  /// <author>  
  /// The MathWorks, Inc. and the National Institute of Standards and Technology.
  /// </author>
  /// <version>  5 August 1998
  /// </version>

  [Serializable]
  public class GeneralMatrix : System.ICloneable, System.Runtime.Serialization.ISerializable, System.IDisposable
  {
    #region Class variables

    /// <summary>Array for internal storage of elements.
    /// @serial internal array storage.
    /// </summary>
    private double[][] a;

    /// <summary>Row and column dimensions.
    /// @serial row dimension.
    /// @serial column dimension.
    /// </summary>
    private int m, n;

    #endregion //  Class variables

    #region Constructors

    /// <summary>Construct an m-by-n matrix of zeros. </summary>
    /// <param name="m">   Number of rows.
    /// </param>
    /// <param name="n">   Number of colums.
    /// </param>
    public GeneralMatrix(int m, int n)
    {
      this.m = m;
      this.n = n;
      a = new double[m][];
      for (int i = 0; i < m; i++)
      {
        a[i] = new double[n];
      }
    }

    /// <summary>Construct an m-by-n constant matrix.</summary>
    /// <param name="m">   Number of rows.
    /// </param>
    /// <param name="n">   Number of colums.
    /// </param>
    /// <param name="s">   Fill the matrix with this scalar value.
    /// </param>
    public GeneralMatrix(int m, int n, double s)
    {
      this.m = m;
      this.n = n;
      a = new double[m][];
      for (int i = 0; i < m; i++)
      {
        a[i] = new double[n];
      }
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          a[i][j] = s;
        }
      }
    }

    /// <summary>Construct a matrix from a 2-D array.</summary>
    /// <param name="A">   Two-dimensional array of doubles.
    /// </param>
    /// <exception cref="System.ArgumentException">   All rows must have the same length
    /// </exception>
    /// <seealso cref="Create">
    /// </seealso>
    public GeneralMatrix(double[][] A)
    {
      m = A.Length;
      n = A[0].Length;
      for (int i = 0; i < m; i++)
      {
        if (A[i].Length != n)
        {
          throw new System.ArgumentException("All rows must have the same length.");
        }
      }
      this.a = A;
    }

    /// <summary>Construct a matrix quickly without checking arguments.</summary>
    /// <param name="A">   Two-dimensional array of doubles.
    /// </param>
    /// <param name="m">   Number of rows.
    /// </param>
    /// <param name="n">   Number of colums.
    /// </param>
    public GeneralMatrix(double[][] A, int m, int n)
    {
      this.a = A;
      this.m = m;
      this.n = n;
    }

    /// <summary>Construct a matrix from a one-dimensional packed array</summary>
    /// <param name="vals">One-dimensional array of doubles, packed by columns (ala Fortran).
    /// </param>
    /// <param name="m">   Number of rows.
    /// </param>
    /// <exception cref="System.ArgumentException">   Array length must be a multiple of m.
    /// </exception>
    public GeneralMatrix(double[] vals, int m)
    {
      this.m = m;
      n = (m != 0 ? vals.Length / m : 0);
      if (m * n != vals.Length)
      {
        throw new System.ArgumentException("Array length must be a multiple of m.");
      }
      a = new double[m][];
      for (int i = 0; i < m; i++)
      {
        a[i] = new double[n];
      }
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          a[i][j] = vals[i + j * m];
        }
      }
    }

    /// <summary>
    /// Deserialization consructor.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected GeneralMatrix(SerializationInfo info, StreamingContext context)
    {
      this.m = info.GetInt32("m");
      this.n = info.GetInt32("n");
      this.a = (double[][])info.GetValue("a", typeof(double[][])); 
      /*
      A = new double[m][];
      for (int i = 0; i < m; i++)
      {
        A[i] = new double[n];
      }
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          A[i][j] = info.GetDouble("a" + i.ToString() +"," + j.ToString());
        }
      }
      */
    }

    #endregion //  Constructors

    #region Public Properties
    /// <summary>Access the internal two-dimensional array.</summary>
    /// <returns>     Pointer to the two-dimensional array of matrix elements.
    /// </returns>
    virtual public double[][] Array
    {
      get
      {
        return a;
      }
    }
    /// <summary>Copy the internal two-dimensional array.</summary>
    /// <returns>     Two-dimensional array copy of matrix elements.
    /// </returns>
    virtual public double[][] ArrayCopy
    {
      get
      {
        double[][] C = new double[m][];
        for (int i = 0; i < m; i++)
        {
          C[i] = new double[n];
        }
        for (int i = 0; i < m; i++)
        {
          for (int j = 0; j < n; j++)
          {
            C[i][j] = a[i][j];
          }
        }
        return C;
      }

    }
    /// <summary>Make a one-dimensional column packed copy of the internal array.</summary>
    /// <returns>     Matrix elements packed in a one-dimensional array by columns.
    /// </returns>
    virtual public double[] ColumnPackedCopy
    {
      get
      {
        double[] vals = new double[m * n];
        for (int i = 0; i < m; i++)
        {
          for (int j = 0; j < n; j++)
          {
            vals[i + j * m] = a[i][j];
          }
        }
        return vals;
      }

    }

    /// <summary>Make a one-dimensional row packed copy of the internal array.</summary>
    /// <returns>     Matrix elements packed in a one-dimensional array by rows.
    /// </returns>
    virtual public double[] RowPackedCopy
    {
      get
      {
        double[] vals = new double[m * n];
        for (int i = 0; i < m; i++)
        {
          for (int j = 0; j < n; j++)
          {
            vals[i * n + j] = a[i][j];
          }
        }
        return vals;
      }
    }

    /// <summary>Get row dimension.</summary>
    /// <returns>     m, the number of rows.
    /// </returns>
    virtual public int RowDimension
    {
      get
      {
        return m;
      }
    }

    /// <summary>Get column dimension.</summary>
    /// <returns>     n, the number of columns.
    /// </returns>
    virtual public int ColumnDimension
    {
      get
      {
        return n;
      }
    }
    #endregion   // Public Properties

    #region	 Public Methods

    /// <summary>Construct a matrix from a copy of a 2-D array.</summary>
    /// <param name="A">   Two-dimensional array of doubles.
    /// </param>
    /// <exception cref="System.ArgumentException">   All rows must have the same length
    /// </exception>

    public static GeneralMatrix Create(double[][] A)
    {
      int m = A.Length;
      int n = A[0].Length;
      GeneralMatrix X = new GeneralMatrix(m, n);
      double[][] C = X.Array;
      for (int i = 0; i < m; i++)
      {
        if (A[i].Length != n)
        {
          throw new System.ArgumentException("All rows must have the same length.");
        }
        for (int j = 0; j < n; j++)
        {
          C[i][j] = A[i][j];
        }
      }
      return X;
    }

    /// <summary>Make a deep copy of a matrix</summary>

    public virtual GeneralMatrix Copy()
    {
      GeneralMatrix X = new GeneralMatrix(m, n);
      double[][] C = X.Array;
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          C[i][j] = a[i][j];
        }
      }
      return X;
    }

    /// <summary>Get a single element.</summary>
    /// <param name="i">   Row index.
    /// </param>
    /// <param name="j">   Column index.
    /// </param>
    /// <returns>     A(i,j)
    /// </returns>
    /// <exception cref="System.IndexOutOfRangeException">  
    /// </exception>

    public virtual double GetElement(int i, int j)
    {
      return a[i][j];
    }

    /// <summary>Get a submatrix.</summary>
    /// <param name="i0">  Initial row index
    /// </param>
    /// <param name="i1">  Final row index
    /// </param>
    /// <param name="j0">  Initial column index
    /// </param>
    /// <param name="j1">  Final column index
    /// </param>
    /// <returns>     A(i0:i1,j0:j1)
    /// </returns>
    /// <exception cref="System.IndexOutOfRangeException">   Submatrix indices
    /// </exception>

    public virtual GeneralMatrix GetMatrix(int i0, int i1, int j0, int j1)
    {
      GeneralMatrix X = new GeneralMatrix(i1 - i0 + 1, j1 - j0 + 1);
      double[][] B = X.Array;
      try
      {
        for (int i = i0; i <= i1; i++)
        {
          for (int j = j0; j <= j1; j++)
          {
            B[i - i0][j - j0] = a[i][j];
          }
        }
      }
      catch (System.IndexOutOfRangeException e)
      {
        throw new System.IndexOutOfRangeException("Submatrix indices", e);
      }
      return X;
    }

    /// <summary>Get a submatrix.</summary>
    /// <param name="r">   Array of row indices.
    /// </param>
    /// <param name="c">   Array of column indices.
    /// </param>
    /// <returns>     A(r(:),c(:))
    /// </returns>
    /// <exception cref="System.IndexOutOfRangeException">   Submatrix indices
    /// </exception>

    public virtual GeneralMatrix GetMatrix(int[] r, int[] c)
    {
      GeneralMatrix X = new GeneralMatrix(r.Length, c.Length);
      double[][] B = X.Array;
      try
      {
        for (int i = 0; i < r.Length; i++)
        {
          for (int j = 0; j < c.Length; j++)
          {
            B[i][j] = a[r[i]][c[j]];
          }
        }
      }
      catch (System.IndexOutOfRangeException e)
      {
        throw new System.IndexOutOfRangeException("Submatrix indices", e);
      }
      return X;
    }

    /// <summary>Get a submatrix.</summary>
    /// <param name="i0">  Initial row index
    /// </param>
    /// <param name="i1">  Final row index
    /// </param>
    /// <param name="c">   Array of column indices.
    /// </param>
    /// <returns>     A(i0:i1,c(:))
    /// </returns>
    /// <exception cref="System.IndexOutOfRangeException">   Submatrix indices
    /// </exception>

    public virtual GeneralMatrix GetMatrix(int i0, int i1, int[] c)
    {
      GeneralMatrix X = new GeneralMatrix(i1 - i0 + 1, c.Length);
      double[][] B = X.Array;
      try
      {
        for (int i = i0; i <= i1; i++)
        {
          for (int j = 0; j < c.Length; j++)
          {
            B[i - i0][j] = a[i][c[j]];
          }
        }
      }
      catch (System.IndexOutOfRangeException e)
      {
        throw new System.IndexOutOfRangeException("Submatrix indices", e);
      }
      return X;
    }

    /// <summary>Get a submatrix.</summary>
    /// <param name="r">   Array of row indices.
    /// </param>
    /// <param name="j0">  Initial column index
    /// </param>
    /// <param name="j1">  Final column index
    /// </param>
    /// <returns>     A(r(:),j0:j1)
    /// </returns>
    /// <exception cref="System.IndexOutOfRangeException">   Submatrix indices
    /// </exception>

    public virtual GeneralMatrix GetMatrix(int[] r, int j0, int j1)
    {
      GeneralMatrix X = new GeneralMatrix(r.Length, j1 - j0 + 1);
      double[][] B = X.Array;
      try
      {
        for (int i = 0; i < r.Length; i++)
        {
          for (int j = j0; j <= j1; j++)
          {
            B[i][j - j0] = a[r[i]][j];
          }
        }
      }
      catch (System.IndexOutOfRangeException e)
      {
        throw new System.IndexOutOfRangeException("Submatrix indices", e);
      }
      return X;
    }

    /// <summary>Set a single element.</summary>
    /// <param name="i">   Row index.
    /// </param>
    /// <param name="j">   Column index.
    /// </param>
    /// <param name="s">   A(i,j).
    /// </param>
    /// <exception cref="System.IndexOutOfRangeException">  
    /// </exception>

    public virtual void SetElement(int i, int j, double s)
    {
      a[i][j] = s;
    }

    /// <summary>Set a submatrix.</summary>
    /// <param name="i0">  Initial row index
    /// </param>
    /// <param name="i1">  Final row index
    /// </param>
    /// <param name="j0">  Initial column index
    /// </param>
    /// <param name="j1">  Final column index
    /// </param>
    /// <param name="X">   A(i0:i1,j0:j1)
    /// </param>
    /// <exception cref="System.IndexOutOfRangeException">  Submatrix indices
    /// </exception>

    public virtual void SetMatrix(int i0, int i1, int j0, int j1, GeneralMatrix X)
    {
      try
      {
        for (int i = i0; i <= i1; i++)
        {
          for (int j = j0; j <= j1; j++)
          {
            a[i][j] = X.GetElement(i - i0, j - j0);
          }
        }
      }
      catch (System.IndexOutOfRangeException e)
      {
        throw new System.IndexOutOfRangeException("Submatrix indices", e);
      }
    }

    /// <summary>Set a submatrix.</summary>
    /// <param name="r">   Array of row indices.
    /// </param>
    /// <param name="c">   Array of column indices.
    /// </param>
    /// <param name="X">   A(r(:),c(:))
    /// </param>
    /// <exception cref="System.IndexOutOfRangeException">  Submatrix indices
    /// </exception>

    public virtual void SetMatrix(int[] r, int[] c, GeneralMatrix X)
    {
      try
      {
        for (int i = 0; i < r.Length; i++)
        {
          for (int j = 0; j < c.Length; j++)
          {
            a[r[i]][c[j]] = X.GetElement(i, j);
          }
        }
      }
      catch (System.IndexOutOfRangeException e)
      {
        throw new System.IndexOutOfRangeException("Submatrix indices", e);
      }
    }

    /// <summary>Set a submatrix.</summary>
    /// <param name="r">   Array of row indices.
    /// </param>
    /// <param name="j0">  Initial column index
    /// </param>
    /// <param name="j1">  Final column index
    /// </param>
    /// <param name="X">   A(r(:),j0:j1)
    /// </param>
    /// <exception cref="System.IndexOutOfRangeException"> Submatrix indices
    /// </exception>

    public virtual void SetMatrix(int[] r, int j0, int j1, GeneralMatrix X)
    {
      try
      {
        for (int i = 0; i < r.Length; i++)
        {
          for (int j = j0; j <= j1; j++)
          {
            a[r[i]][j] = X.GetElement(i, j - j0);
          }
        }
      }
      catch (System.IndexOutOfRangeException e)
      {
        throw new System.IndexOutOfRangeException("Submatrix indices", e);
      }
    }

    /// <summary>Set a submatrix.</summary>
    /// <param name="i0">  Initial row index
    /// </param>
    /// <param name="i1">  Final row index
    /// </param>
    /// <param name="c">   Array of column indices.
    /// </param>
    /// <param name="X">   A(i0:i1,c(:))
    /// </param>
    /// <exception cref="System.IndexOutOfRangeException">  Submatrix indices
    /// </exception>

    public virtual void SetMatrix(int i0, int i1, int[] c, GeneralMatrix X)
    {
      try
      {
        for (int i = i0; i <= i1; i++)
        {
          for (int j = 0; j < c.Length; j++)
          {
            a[i][c[j]] = X.GetElement(i - i0, j);
          }
        }
      }
      catch (System.IndexOutOfRangeException e)
      {
        throw new System.IndexOutOfRangeException("Submatrix indices", e);
      }
    }

    /// <summary>Matrix transpose.</summary>
    /// <returns>    A'
    /// </returns>

    public virtual GeneralMatrix Transpose()
    {
      GeneralMatrix X = new GeneralMatrix(n, m);
      double[][] C = X.Array;
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          C[j][i] = a[i][j];
        }
      }
      return X;
    }

    /// <summary>One norm</summary>
    /// <returns>    maximum column sum.
    /// </returns>

    public virtual double Norm1()
    {
      double f = 0;
      for (int j = 0; j < n; j++)
      {
        double s = 0;
        for (int i = 0; i < m; i++)
        {
          s += System.Math.Abs(a[i][j]);
        }
        f = System.Math.Max(f, s);
      }
      return f;
    }

    /// <summary>Two norm</summary>
    /// <returns>    maximum singular value.
    /// </returns>

    public virtual double Norm2()
    {
      return (new SingularValueDecomposition(this).Norm2());
    }

    /// <summary>Infinity norm</summary>
    /// <returns>    maximum row sum.
    /// </returns>

    public virtual double NormInf()
    {
      double f = 0;
      for (int i = 0; i < m; i++)
      {
        double s = 0;
        for (int j = 0; j < n; j++)
        {
          s += System.Math.Abs(a[i][j]);
        }
        f = System.Math.Max(f, s);
      }
      return f;
    }

    /// <summary>Frobenius norm</summary>
    /// <returns>    sqrt of sum of squares of all elements.
    /// </returns>

    public virtual double NormF()
    {
      double f = 0;
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          f = Maths.Hypot(f, a[i][j]);
        }
      }
      return f;
    }

    /// <summary>Unary minus</summary>
    /// <returns>    -A
    /// </returns>

    public virtual GeneralMatrix UnaryMinus()
    {
      GeneralMatrix X = new GeneralMatrix(m, n);
      double[][] C = X.Array;
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          C[i][j] = -a[i][j];
        }
      }
      return X;
    }

    /// <summary>C = A + B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     A + B
    /// </returns>

    public virtual GeneralMatrix Add(GeneralMatrix B)
    {
      CheckMatrixDimensions(B);
      GeneralMatrix X = new GeneralMatrix(m, n);
      double[][] C = X.Array;
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          C[i][j] = a[i][j] + B.a[i][j];
        }
      }
      return X;
    }

    /// <summary>A = A + B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     A + B
    /// </returns>

    public virtual GeneralMatrix AddEquals(GeneralMatrix B)
    {
      CheckMatrixDimensions(B);
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          a[i][j] = a[i][j] + B.a[i][j];
        }
      }
      return this;
    }

    /// <summary>C = A - B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     A - B
    /// </returns>

    public virtual GeneralMatrix Subtract(GeneralMatrix B)
    {
      CheckMatrixDimensions(B);
      GeneralMatrix X = new GeneralMatrix(m, n);
      double[][] C = X.Array;
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          C[i][j] = a[i][j] - B.a[i][j];
        }
      }
      return X;
    }

    /// <summary>A = A - B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     A - B
    /// </returns>

    public virtual GeneralMatrix SubtractEquals(GeneralMatrix B)
    {
      CheckMatrixDimensions(B);
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          a[i][j] = a[i][j] - B.a[i][j];
        }
      }
      return this;
    }

    /// <summary>Element-by-element multiplication, C = A.*B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     A.*B
    /// </returns>

    public virtual GeneralMatrix ArrayMultiply(GeneralMatrix B)
    {
      CheckMatrixDimensions(B);
      GeneralMatrix X = new GeneralMatrix(m, n);
      double[][] C = X.Array;
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          C[i][j] = a[i][j] * B.a[i][j];
        }
      }
      return X;
    }

    /// <summary>Element-by-element multiplication in place, A = A.*B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     A.*B
    /// </returns>

    public virtual GeneralMatrix ArrayMultiplyEquals(GeneralMatrix B)
    {
      CheckMatrixDimensions(B);
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          a[i][j] = a[i][j] * B.a[i][j];
        }
      }
      return this;
    }

    /// <summary>Element-by-element right division, C = A./B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     A./B
    /// </returns>

    public virtual GeneralMatrix ArrayRightDivide(GeneralMatrix B)
    {
      CheckMatrixDimensions(B);
      GeneralMatrix X = new GeneralMatrix(m, n);
      double[][] C = X.Array;
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          C[i][j] = a[i][j] / B.a[i][j];
        }
      }
      return X;
    }

    /// <summary>Element-by-element right division in place, A = A./B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     A./B
    /// </returns>

    public virtual GeneralMatrix ArrayRightDivideEquals(GeneralMatrix B)
    {
      CheckMatrixDimensions(B);
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          a[i][j] = a[i][j] / B.a[i][j];
        }
      }
      return this;
    }

    /// <summary>Element-by-element left division, C = A.\B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     A.\B
    /// </returns>

    public virtual GeneralMatrix ArrayLeftDivide(GeneralMatrix B)
    {
      CheckMatrixDimensions(B);
      GeneralMatrix X = new GeneralMatrix(m, n);
      double[][] C = X.Array;
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          C[i][j] = B.a[i][j] / a[i][j];
        }
      }
      return X;
    }

    /// <summary>Element-by-element left division in place, A = A.\B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     A.\B
    /// </returns>

    public virtual GeneralMatrix ArrayLeftDivideEquals(GeneralMatrix B)
    {
      CheckMatrixDimensions(B);
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          a[i][j] = B.a[i][j] / a[i][j];
        }
      }
      return this;
    }

    /// <summary>Multiply a matrix by a scalar, C = s*A</summary>
    /// <param name="s">   scalar
    /// </param>
    /// <returns>     s*A
    /// </returns>

    public virtual GeneralMatrix Multiply(double s)
    {
      GeneralMatrix X = new GeneralMatrix(m, n);
      double[][] C = X.Array;
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          C[i][j] = s * a[i][j];
        }
      }
      return X;
    }

    /// <summary>Multiply a matrix by a scalar in place, A = s*A</summary>
    /// <param name="s">   scalar
    /// </param>
    /// <returns>     replace A by s*A
    /// </returns>

    public virtual GeneralMatrix MultiplyEquals(double s)
    {
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          a[i][j] = s * a[i][j];
        }
      }
      return this;
    }

    /// <summary>Linear algebraic matrix multiplication, A * B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     Matrix product, A * B
    /// </returns>
    /// <exception cref="System.ArgumentException">  Matrix inner dimensions must agree.
    /// </exception>

    public virtual GeneralMatrix Multiply(GeneralMatrix B)
    {
      if (B.m != n)
      {
        throw new System.ArgumentException("GeneralMatrix inner dimensions must agree.");
      }
      GeneralMatrix X = new GeneralMatrix(m, B.n);
      double[][] C = X.Array;
      double[] Bcolj = new double[n];
      for (int j = 0; j < B.n; j++)
      {
        for (int k = 0; k < n; k++)
        {
          Bcolj[k] = B.a[k][j];
        }
        for (int i = 0; i < m; i++)
        {
          double[] Arowi = a[i];
          double s = 0;
          for (int k = 0; k < n; k++)
          {
            s += Arowi[k] * Bcolj[k];
          }
          C[i][j] = s;
        }
      }
      return X;
    }

    #region Operator Overloading

    /// <summary>
    ///  Addition of matrices
    /// </summary>
    /// <param name="m1"></param>
    /// <param name="m2"></param>
    /// <returns></returns>
    public static GeneralMatrix operator +(GeneralMatrix m1, GeneralMatrix m2)
    {
      return m1.Add(m2);
    }

    /// <summary>
    /// Subtraction of matrices
    /// </summary>
    /// <param name="m1"></param>
    /// <param name="m2"></param>
    /// <returns></returns>
    public static GeneralMatrix operator -(GeneralMatrix m1, GeneralMatrix m2)
    {
      return m1.Subtract(m2);
    }

    /// <summary>
    /// Multiplication of matrices
    /// </summary>
    /// <param name="m1"></param>
    /// <param name="m2"></param>
    /// <returns></returns>
    public static GeneralMatrix operator *(GeneralMatrix m1, GeneralMatrix m2)
    {
      return m1.Multiply(m2);
    }

    #endregion   //Operator Overloading

    /// <summary>LU Decomposition</summary>
    /// <returns>     LUDecomposition
    /// </returns>
    /// <seealso cref="LUDecomposition">
    /// </seealso>

    public virtual LUDecomposition LUD()
    {
      return new LUDecomposition(this);
    }

    /// <summary>QR Decomposition</summary>
    /// <returns>     QRDecomposition
    /// </returns>
    /// <seealso cref="QRDecomposition">
    /// </seealso>

    public virtual QRDecomposition QRD()
    {
      return new QRDecomposition(this);
    }

    /// <summary>Cholesky Decomposition</summary>
    /// <returns>     CholeskyDecomposition
    /// </returns>
    /// <seealso cref="CholeskyDecomposition">
    /// </seealso>

    public virtual CholeskyDecomposition chol()
    {
      return new CholeskyDecomposition(this);
    }

    /// <summary>Singular Value Decomposition</summary>
    /// <returns>     SingularValueDecomposition
    /// </returns>
    /// <seealso cref="SingularValueDecomposition">
    /// </seealso>

    public virtual SingularValueDecomposition SVD()
    {
      return new SingularValueDecomposition(this);
    }

    /// <summary>Eigenvalue Decomposition</summary>
    /// <returns>     EigenvalueDecomposition
    /// </returns>
    /// <seealso cref="EigenvalueDecomposition">
    /// </seealso>

    public virtual EigenvalueDecomposition Eigen()
    {
      return new EigenvalueDecomposition(this);
    }

    /// <summary>Solve A*X = B</summary>
    /// <param name="B">   right hand side
    /// </param>
    /// <returns>     solution if A is square, least squares solution otherwise
    /// </returns>

    public virtual GeneralMatrix Solve(GeneralMatrix B)
    {
      return (m == n ? (new LUDecomposition(this)).Solve(B) : (new QRDecomposition(this)).Solve(B));
    }

    /// <summary>Solve X*A = B, which is also A'*X' = B'</summary>
    /// <param name="B">   right hand side
    /// </param>
    /// <returns>     solution if A is square, least squares solution otherwise.
    /// </returns>

    public virtual GeneralMatrix SolveTranspose(GeneralMatrix B)
    {
      return Transpose().Solve(B.Transpose());
    }

    /// <summary>Matrix inverse or pseudoinverse</summary>
    /// <returns>     inverse(A) if A is square, pseudoinverse otherwise.
    /// </returns>

    public virtual GeneralMatrix Inverse()
    {
      return Solve(Identity(m, m));
    }

    /// <summary>GeneralMatrix determinant</summary>
    /// <returns>     determinant
    /// </returns>

    public virtual double Determinant()
    {
      return new LUDecomposition(this).Determinant();
    }

    /// <summary>GeneralMatrix rank</summary>
    /// <returns>     effective numerical rank, obtained from SVD.
    /// </returns>

    public virtual int Rank()
    {
      return new SingularValueDecomposition(this).Rank();
    }

    /// <summary>Matrix condition (2 norm)</summary>
    /// <returns>     ratio of largest to smallest singular value.
    /// </returns>

    public virtual double Condition()
    {
      return new SingularValueDecomposition(this).Condition();
    }

    /// <summary>Matrix trace.</summary>
    /// <returns>     sum of the diagonal elements.
    /// </returns>

    public virtual double Trace()
    {
      double t = 0;
      for (int i = 0; i < System.Math.Min(m, n); i++)
      {
        t += a[i][i];
      }
      return t;
    }

    /// <summary>Generate matrix with random elements</summary>
    /// <param name="m">   Number of rows.
    /// </param>
    /// <param name="n">   Number of colums.
    /// </param>
    /// <returns>     An m-by-n matrix with uniformly distributed random elements.
    /// </returns>

    public static GeneralMatrix Random(int m, int n)
    {
      System.Random random = new System.Random();

      GeneralMatrix A = new GeneralMatrix(m, n);
      double[][] X = A.Array;
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          X[i][j] = random.NextDouble();
        }
      }
      return A;
    }

    /// <summary>Generate identity matrix</summary>
    /// <param name="m">   Number of rows.
    /// </param>
    /// <param name="n">   Number of colums.
    /// </param>
    /// <returns>     An m-by-n matrix with ones on the diagonal and zeros elsewhere.
    /// </returns>

    public static GeneralMatrix Identity(int m, int n)
    {
      GeneralMatrix A = new GeneralMatrix(m, n);
      double[][] X = A.Array;
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          X[i][j] = (i == j ? 1.0 : 0.0);
        }
      }
      return A;
    }

    #endregion //  Public Methods

    #region	 Private Methods

    /// <summary>Check if size(A) == size(B) *</summary>

    private void CheckMatrixDimensions(GeneralMatrix B)
    {
      if (B.m != m || B.n != n)
      {
        throw new System.ArgumentException("GeneralMatrix dimensions must agree.");
      }
    }
    #endregion //  Private Methods

    #region Implement IDisposable
    /// <summary>
    /// Do not make this method virtual.
    /// A derived class should not be able to override this method.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
    }

    /// <summary>
    /// Dispose(bool disposing) executes in two distinct scenarios.
    /// If disposing equals true, the method has been called directly
    /// or indirectly by a user's code. Managed and unmanaged resources
    /// can be disposed.
    /// If disposing equals false, the method has been called by the 
    /// runtime from inside the finalizer and you should not reference 
    /// other objects. Only unmanaged resources can be disposed.
    /// </summary>
    /// <param name="disposing"></param>
    private void Dispose(bool disposing)
    {
      // This object will be cleaned up by the Dispose method.
      // Therefore, you should call GC.SupressFinalize to
      // take this object off the finalization queue 
      // and prevent finalization code for this object
      // from executing a second time.
      if (disposing)
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// This destructor will run only if the Dispose method 
    /// does not get called.
    /// It gives your base class the opportunity to finalize.
    /// Do not provide destructors in types derived from this class.
    /// </summary>
    ~GeneralMatrix()
    {
      // Do not re-create Dispose clean-up code here.
      // Calling Dispose(false) is optimal in terms of
      // readability and maintainability.
      Dispose(false);
    }
    #endregion //  Implement IDisposable

    /// <summary>Clone the GeneralMatrix object.</summary>
    public System.Object Clone()
    {
      return this.Copy();
    }

    /// <summary>
    /// A method called when serializing this class
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("m", m);
      info.AddValue("n", n);
      info.AddValue("a", a);
    }
  }

  /// <summary>LU Decomposition.
  /// For an m-by-n matrix A with m >= n, the LU decomposition is an m-by-n
  /// unit lower triangular matrix L, an n-by-n upper triangular matrix U,
  /// and a permutation vector piv of length m so that A(piv,:) = L*U.
  /// <code> If m < n, then L is m-by-m and U is m-by-n. </code>
  /// The LU decompostion with pivoting always exists, even if the matrix is
  /// singular, so the constructor will never fail.  The primary use of the
  /// LU decomposition is in the solution of square systems of simultaneous
  /// linear equations.  This will fail if IsNonSingular() returns false.
  /// </summary>

  [Serializable]
  public class LUDecomposition : System.Runtime.Serialization.ISerializable
  {
    #region Class variables

    /// <summary>Array for internal storage of decomposition.
    /// @serial internal array storage.
    /// </summary>
    private double[][] lu;

    /// <summary>Row and column dimensions, and pivot sign.
    /// @serial column dimension.
    /// @serial row dimension.
    /// @serial pivot sign.
    /// </summary>
    private int m, n, pivsign;

    /// <summary>Internal storage of pivot vector.
    /// @serial pivot vector.
    /// </summary>
    private int[] piv;

    #endregion //  Class variables

    #region Constructor

    /// <summary>LU Decomposition</summary>
    /// <param name="A">  Rectangular matrix
    /// </param>
    /// <returns>     Structure to access L, U and piv.
    /// </returns>

    public LUDecomposition(GeneralMatrix A)
    {
      // Use a "left-looking", dot-product, Crout/Doolittle algorithm.

      lu = A.ArrayCopy;
      m = A.RowDimension;
      n = A.ColumnDimension;
      piv = new int[m];
      for (int i = 0; i < m; i++)
      {
        piv[i] = i;
      }
      pivsign = 1;
      double[] LUrowi;
      double[] LUcolj = new double[m];

      // Outer loop.

      for (int j = 0; j < n; j++)
      {

        // Make a copy of the j-th column to localize references.

        for (int i = 0; i < m; i++)
        {
          LUcolj[i] = lu[i][j];
        }

        // Apply previous transformations.

        for (int i = 0; i < m; i++)
        {
          LUrowi = lu[i];

          // Most of the time is spent in the following dot product.

          int kmax = System.Math.Min(i, j);
          double s = 0.0;
          for (int k = 0; k < kmax; k++)
          {
            s += LUrowi[k] * LUcolj[k];
          }

          LUrowi[j] = LUcolj[i] -= s;
        }

        // Find pivot and exchange if necessary.

        int p = j;
        for (int i = j + 1; i < m; i++)
        {
          if (System.Math.Abs(LUcolj[i]) > System.Math.Abs(LUcolj[p]))
          {
            p = i;
          }
        }
        if (p != j)
        {
          for (int k = 0; k < n; k++)
          {
            double t = lu[p][k]; lu[p][k] = lu[j][k]; lu[j][k] = t;
          }
          int k2 = piv[p]; piv[p] = piv[j]; piv[j] = k2;
          pivsign = -pivsign;
        }

        // Compute multipliers.

        if (j < m & lu[j][j] != 0.0)
        {
          for (int i = j + 1; i < m; i++)
          {
            lu[i][j] /= lu[j][j];
          }
        }
      }
    }
    #endregion //  Constructor

    #region Public Properties
    /// <summary>Is the matrix nonsingular?</summary>
    /// <returns>     true if U, and hence A, is nonsingular.
    /// </returns>
    virtual public bool IsNonSingular
    {
      get
      {
        for (int j = 0; j < n; j++)
        {
          if (lu[j][j] == 0)
            return false;
        }
        return true;
      }
    }

    /// <summary>Return lower triangular factor</summary>
    /// <returns>     L
    /// </returns>
    virtual public GeneralMatrix L
    {
      get
      {
        GeneralMatrix X = new GeneralMatrix(m, n);
        double[][] L = X.Array;
        for (int i = 0; i < m; i++)
        {
          for (int j = 0; j < n; j++)
          {
            if (i > j)
            {
              L[i][j] = lu[i][j];
            }
            else if (i == j)
            {
              L[i][j] = 1.0;
            }
            else
            {
              L[i][j] = 0.0;
            }
          }
        }
        return X;
      }
    }

    /// <summary>Return upper triangular factor</summary>
    /// <returns>     U
    /// </returns>
    virtual public GeneralMatrix U
    {
      get
      {
        GeneralMatrix X = new GeneralMatrix(n, n);
        double[][] U = X.Array;
        for (int i = 0; i < n; i++)
        {
          for (int j = 0; j < n; j++)
          {
            if (i <= j)
            {
              U[i][j] = lu[i][j];
            }
            else
            {
              U[i][j] = 0.0;
            }
          }
        }
        return X;
      }
    }

    /// <summary>Return pivot permutation vector</summary>
    /// <returns>     piv
    /// </returns>
    virtual public int[] Pivot
    {
      get
      {
        int[] p = new int[m];
        for (int i = 0; i < m; i++)
        {
          p[i] = piv[i];
        }
        return p;
      }
    }

    /// <summary>Return pivot permutation vector as a one-dimensional double array</summary>
    /// <returns>     (double) piv
    /// </returns>
    virtual public double[] DoublePivot
    {
      get
      {
        double[] vals = new double[m];
        for (int i = 0; i < m; i++)
        {
          vals[i] = (double)piv[i];
        }
        return vals;
      }
    }

    #endregion //  Public Properties

    #region Public Methods

    /// <summary>Determinant</summary>
    /// <returns>     det(A)
    /// </returns>
    /// <exception cref="System.ArgumentException">  Matrix must be square
    /// </exception>

    public virtual double Determinant()
    {
      if (m != n)
      {
        throw new System.ArgumentException("Matrix must be square.");
      }
      double d = (double)pivsign;
      for (int j = 0; j < n; j++)
      {
        d *= lu[j][j];
      }
      return d;
    }

    /// <summary>Solve A*X = B</summary>
    /// <param name="B">  A Matrix with as many rows as A and any number of columns.
    /// </param>
    /// <returns>     X so that L*U*X = B(piv,:)
    /// </returns>
    /// <exception cref="System.ArgumentException"> Matrix row dimensions must agree.
    /// </exception>
    /// <exception cref="System.SystemException"> Matrix is singular.
    /// </exception>

    public virtual GeneralMatrix Solve(GeneralMatrix B)
    {
      if (B.RowDimension != m)
      {
        throw new System.ArgumentException("Matrix row dimensions must agree.");
      }
      if (!this.IsNonSingular)
      {
        throw new System.SystemException("Matrix is singular.");
      }

      // Copy right hand side with pivoting
      int nx = B.ColumnDimension;
      GeneralMatrix Xmat = B.GetMatrix(piv, 0, nx - 1);
      double[][] X = Xmat.Array;

      // Solve L*Y = B(piv,:)
      for (int k = 0; k < n; k++)
      {
        for (int i = k + 1; i < n; i++)
        {
          for (int j = 0; j < nx; j++)
          {
            X[i][j] -= X[k][j] * lu[i][k];
          }
        }
      }
      // Solve U*X = Y;
      for (int k = n - 1; k >= 0; k--)
      {
        for (int j = 0; j < nx; j++)
        {
          X[k][j] /= lu[k][k];
        }
        for (int i = 0; i < k; i++)
        {
          for (int j = 0; j < nx; j++)
          {
            X[i][j] -= X[k][j] * lu[i][k];
          }
        }
      }
      return Xmat;
    }

    #endregion //  Public Methods

    // A method called when serializing this class.
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
    }
  }

  /// <summary>QR Decomposition.
  /// For an m-by-n matrix A with m >= n, the QR decomposition is an m-by-n
  /// orthogonal matrix Q and an n-by-n upper triangular matrix R so that
  /// A = Q*R.
  /// 
  /// The QR decompostion always exists, even if the matrix does not have
  /// full rank, so the constructor will never fail.  The primary use of the
  /// QR decomposition is in the least squares solution of nonsquare systems
  /// of simultaneous linear equations.  This will fail if IsFullRank()
  /// returns false.
  /// </summary>

  [Serializable]
  public class QRDecomposition : System.Runtime.Serialization.ISerializable
  {
    #region Class variables

    /// <summary>Array for internal storage of decomposition.
    /// @serial internal array storage.
    /// </summary>
    private double[][] qr;

    /// <summary>Row and column dimensions.
    /// @serial column dimension.
    /// @serial row dimension.
    /// </summary>
    private int m, n;

    /// <summary>Array for internal storage of diagonal of R.
    /// @serial diagonal of R.
    /// </summary>
    private double[] rDiag;

    #endregion //  Class variables

    #region Constructor

    /// <summary>QR Decomposition, computed by Householder reflections.</summary>
    /// <param name="A">   Rectangular matrix
    /// </param>
    /// <returns>     Structure to access R and the Householder vectors and compute Q.
    /// </returns>

    public QRDecomposition(GeneralMatrix A)
    {
      // Initialize.
      qr = A.ArrayCopy;
      m = A.RowDimension;
      n = A.ColumnDimension;
      rDiag = new double[n];

      // Main loop.
      for (int k = 0; k < n; k++)
      {
        // Compute 2-norm of k-th column without under/overflow.
        double nrm = 0;
        for (int i = k; i < m; i++)
        {
          nrm = Maths.Hypot(nrm, qr[i][k]);
        }

        if (nrm != 0.0)
        {
          // Form k-th Householder vector.
          if (qr[k][k] < 0)
          {
            nrm = -nrm;
          }
          for (int i = k; i < m; i++)
          {
            qr[i][k] /= nrm;
          }
          qr[k][k] += 1.0;

          // Apply transformation to remaining columns.
          for (int j = k + 1; j < n; j++)
          {
            double s = 0.0;
            for (int i = k; i < m; i++)
            {
              s += qr[i][k] * qr[i][j];
            }
            s = (-s) / qr[k][k];
            for (int i = k; i < m; i++)
            {
              qr[i][j] += s * qr[i][k];
            }
          }
        }
        rDiag[k] = -nrm;
      }
    }

    #endregion //  Constructor

    #region Public Properties

    /// <summary>Is the matrix full rank?</summary>
    /// <returns>     true if R, and hence A, has full rank.
    /// </returns>
    virtual public bool FullRank
    {
      get
      {
        for (int j = 0; j < n; j++)
        {
          if (rDiag[j] == 0)
            return false;
        }
        return true;
      }
    }

    /// <summary>Return the Householder vectors</summary>
    /// <returns>     Lower trapezoidal matrix whose columns define the reflections
    /// </returns>
    virtual public GeneralMatrix H
    {
      get
      {
        GeneralMatrix X = new GeneralMatrix(m, n);
        double[][] H = X.Array;
        for (int i = 0; i < m; i++)
        {
          for (int j = 0; j < n; j++)
          {
            if (i >= j)
            {
              H[i][j] = qr[i][j];
            }
            else
            {
              H[i][j] = 0.0;
            }
          }
        }
        return X;
      }

    }

    /// <summary>Return the upper triangular factor</summary>
    /// <returns>     R
    /// </returns>
    virtual public GeneralMatrix R
    {
      get
      {
        GeneralMatrix X = new GeneralMatrix(n, n);
        double[][] R = X.Array;
        for (int i = 0; i < n; i++)
        {
          for (int j = 0; j < n; j++)
          {
            if (i < j)
            {
              R[i][j] = qr[i][j];
            }
            else if (i == j)
            {
              R[i][j] = rDiag[i];
            }
            else
            {
              R[i][j] = 0.0;
            }
          }
        }
        return X;
      }
    }

    /// <summary>Generate and return the (economy-sized) orthogonal factor</summary>
    /// <returns>     Q
    /// </returns>
    virtual public GeneralMatrix Q
    {
      get
      {
        GeneralMatrix X = new GeneralMatrix(m, n);
        double[][] Q = X.Array;
        for (int k = n - 1; k >= 0; k--)
        {
          for (int i = 0; i < m; i++)
          {
            Q[i][k] = 0.0;
          }
          Q[k][k] = 1.0;
          for (int j = k; j < n; j++)
          {
            if (qr[k][k] != 0)
            {
              double s = 0.0;
              for (int i = k; i < m; i++)
              {
                s += qr[i][k] * Q[i][j];
              }
              s = (-s) / qr[k][k];
              for (int i = k; i < m; i++)
              {
                Q[i][j] += s * qr[i][k];
              }
            }
          }
        }
        return X;
      }
    }
    #endregion //  Public Properties

    #region Public Methods

    /// <summary>Least squares solution of A*X = B</summary>
    /// <param name="B">   A Matrix with as many rows as A and any number of columns.
    /// </param>
    /// <returns>     X that minimizes the two norm of Q*R*X-B.
    /// </returns>
    /// <exception cref="System.ArgumentException"> Matrix row dimensions must agree.
    /// </exception>
    /// <exception cref="System.SystemException"> Matrix is rank deficient.
    /// </exception>

    public virtual GeneralMatrix Solve(GeneralMatrix B)
    {
      if (B.RowDimension != m)
      {
        throw new System.ArgumentException("GeneralMatrix row dimensions must agree.");
      }
      if (!this.FullRank)
      {
        throw new System.SystemException("Matrix is rank deficient.");
      }

      // Copy right hand side
      int nx = B.ColumnDimension;
      double[][] X = B.ArrayCopy;

      // Compute Y = transpose(Q)*B
      for (int k = 0; k < n; k++)
      {
        for (int j = 0; j < nx; j++)
        {
          double s = 0.0;
          for (int i = k; i < m; i++)
          {
            s += qr[i][k] * X[i][j];
          }
          s = (-s) / qr[k][k];
          for (int i = k; i < m; i++)
          {
            X[i][j] += s * qr[i][k];
          }
        }
      }
      // Solve R*X = Y;
      for (int k = n - 1; k >= 0; k--)
      {
        for (int j = 0; j < nx; j++)
        {
          X[k][j] /= rDiag[k];
        }
        for (int i = 0; i < k; i++)
        {
          for (int j = 0; j < nx; j++)
          {
            X[i][j] -= X[k][j] * qr[i][k];
          }
        }
      }

      return (new GeneralMatrix(X, n, nx).GetMatrix(0, n - 1, 0, nx - 1));
    }

    #endregion //  Public Methods

    // A method called when serializing this class.
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
    }
  }

  /// <summary>Cholesky Decomposition.
  /// For a symmetric, positive definite matrix A, the Cholesky decomposition
  /// is an lower triangular matrix L so that A = L*L'.
  /// If the matrix is not symmetric or positive definite, the constructor
  /// returns a partial decomposition and sets an internal flag that may
  /// be queried by the isSPD() method.
  /// </summary>

  [Serializable]
  public class CholeskyDecomposition : System.Runtime.Serialization.ISerializable
  {
    #region Class variables

    /// <summary>Array for internal storage of decomposition.
    /// @serial internal array storage.
    /// </summary>
    private double[][] l;

    /// <summary>Row and column dimension (square matrix).
    /// @serial matrix dimension.
    /// </summary>
    private int n;

    /// <summary>Symmetric and positive definite flag.
    /// @serial is symmetric and positive definite flag.
    /// </summary>
    private bool isspd;

    #endregion //  Class variables

    #region Constructor

    /// <summary>Cholesky algorithm for symmetric and positive definite matrix.</summary>
    /// <param name="Arg">  Square, symmetric matrix.
    /// </param>
    /// <returns>     Structure to access L and isspd flag.
    /// </returns>

    public CholeskyDecomposition(GeneralMatrix Arg)
    {
      // Initialize.
      double[][] A = Arg.Array;
      n = Arg.RowDimension;
      l = new double[n][];
      for (int i = 0; i < n; i++)
      {
        l[i] = new double[n];
      }
      isspd = (Arg.ColumnDimension == n);
      // Main loop.
      for (int j = 0; j < n; j++)
      {
        double[] Lrowj = l[j];
        double d = 0.0;
        for (int k = 0; k < j; k++)
        {
          double[] Lrowk = l[k];
          double s = 0.0;
          for (int i = 0; i < k; i++)
          {
            s += Lrowk[i] * Lrowj[i];
          }
          Lrowj[k] = s = (A[j][k] - s) / l[k][k];
          d = d + s * s;
          isspd = isspd & (A[k][j] == A[j][k]);
        }
        d = A[j][j] - d;
        isspd = isspd & (d > 0.0);
        l[j][j] = System.Math.Sqrt(System.Math.Max(d, 0.0));
        for (int k = j + 1; k < n; k++)
        {
          l[j][k] = 0.0;
        }
      }
    }

    #endregion //  Constructor

    #region Public Properties
    /// <summary>Is the matrix symmetric and positive definite?</summary>
    /// <returns>     true if A is symmetric and positive definite.
    /// </returns>
    virtual public bool SPD
    {
      get
      {
        return isspd;
      }
    }
    #endregion   // Public Properties

    #region Public Methods

    /// <summary>Return triangular factor.</summary>
    /// <returns>     L
    /// </returns>

    public virtual GeneralMatrix GetL()
    {
      return new GeneralMatrix(l, n, n);
    }

    /// <summary>Solve A*X = B</summary>
    /// <param name="B">  A Matrix with as many rows as A and any number of columns.
    /// </param>
    /// <returns>     X so that L*L'*X = B
    /// </returns>
    /// <exception cref="System.ArgumentException">  Matrix row dimensions must agree.
    /// </exception>
    /// <exception cref="System.SystemException"> Matrix is not symmetric positive definite.
    /// </exception>

    public virtual GeneralMatrix Solve(GeneralMatrix B)
    {
      if (B.RowDimension != n)
      {
        throw new System.ArgumentException("Matrix row dimensions must agree.");
      }
      if (!isspd)
      {
        throw new System.SystemException("Matrix is not symmetric positive definite.");
      }

      // Copy right hand side.
      double[][] X = B.ArrayCopy;
      int nx = B.ColumnDimension;

      // Solve L*Y = B;
      for (int k = 0; k < n; k++)
      {
        for (int i = k + 1; i < n; i++)
        {
          for (int j = 0; j < nx; j++)
          {
            X[i][j] -= X[k][j] * l[i][k];
          }
        }
        for (int j = 0; j < nx; j++)
        {
          X[k][j] /= l[k][k];
        }
      }

      // Solve L'*X = Y;
      for (int k = n - 1; k >= 0; k--)
      {
        for (int j = 0; j < nx; j++)
        {
          X[k][j] /= l[k][k];
        }
        for (int i = 0; i < k; i++)
        {
          for (int j = 0; j < nx; j++)
          {
            X[i][j] -= X[k][j] * l[k][i];
          }
        }
      }
      return new GeneralMatrix(X, n, nx);
    }
    #endregion //  Public Methods

    // A method called when serializing this class.
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
    }
  }

  /// <summary>Eigenvalues and eigenvectors of a real matrix. 
  /// If A is symmetric, then A = V*D*V' where the eigenvalue matrix D is
  /// diagonal and the eigenvector matrix V is orthogonal.
  /// I.e. A = V.Multiply(D.Multiply(V.Transpose())) and 
  /// V.Multiply(V.Transpose()) equals the identity matrix.
  /// If A is not symmetric, then the eigenvalue matrix D is block diagonal
  /// with the real eigenvalues in 1-by-1 blocks and any complex eigenvalues,
  /// lambda + i*mu, in 2-by-2 blocks, [lambda, mu; -mu, lambda].  The
  /// columns of V represent the eigenvectors in the sense that A*V = V*D,
  /// i.e. A.Multiply(V) equals V.Multiply(D).  The matrix V may be badly
  /// conditioned, or even singular, so the validity of the equation
  /// A = V*D*Inverse(V) depends upon V.cond().
  /// 
  /// </summary>

  [Serializable]
  public class EigenvalueDecomposition : System.Runtime.Serialization.ISerializable
  {
    #region	 Class variables

    /// <summary>Row and column dimension (square matrix).
    /// @serial matrix dimension.
    /// </summary>
    private int n;

    /// <summary>Symmetry flag.
    /// @serial internal symmetry flag.
    /// </summary>
    private bool issymmetric;

    /// <summary>Arrays for internal storage of eigenvalues.
    /// @serial internal storage of eigenvalues.
    /// </summary>
    private double[] d, e;

    /// <summary>Array for internal storage of eigenvectors.
    /// @serial internal storage of eigenvectors.
    /// </summary>
    private double[][] v;

    /// <summary>Array for internal storage of nonsymmetric Hessenberg form.
    /// @serial internal storage of nonsymmetric Hessenberg form.
    /// </summary>
    private double[][] H;

    /// <summary>Working storage for nonsymmetric algorithm.
    /// @serial working storage for nonsymmetric algorithm.
    /// </summary>
    private double[] ort;

    #endregion //  Class variables

    #region Private Methods

    // Symmetric Householder reduction to tridiagonal form.

    private void tred2()
    {
      //  This is derived from the Algol procedures tred2 by
      //  Bowdler, Martin, Reinsch, and Wilkinson, Handbook for
      //  Auto. Comp., Vol.ii-Linear Algebra, and the corresponding
      //  Fortran subroutine in EISPACK.

      for (int j = 0; j < n; j++)
      {
        d[j] = v[n - 1][j];
      }

      // Householder reduction to tridiagonal form.

      for (int i = n - 1; i > 0; i--)
      {
        // Scale to avoid under/overflow.

        double scale = 0.0;
        double h = 0.0;
        for (int k = 0; k < i; k++)
        {
          scale = scale + System.Math.Abs(d[k]);
        }
        if (scale == 0.0)
        {
          e[i] = d[i - 1];
          for (int j = 0; j < i; j++)
          {
            d[j] = v[i - 1][j];
            v[i][j] = 0.0;
            v[j][i] = 0.0;
          }
        }
        else
        {
          // Generate Householder vector.

          for (int k = 0; k < i; k++)
          {
            d[k] /= scale;
            h += d[k] * d[k];
          }
          double f = d[i - 1];
          double g = System.Math.Sqrt(h);
          if (f > 0)
          {
            g = -g;
          }
          e[i] = scale * g;
          h = h - f * g;
          d[i - 1] = f - g;
          for (int j = 0; j < i; j++)
          {
            e[j] = 0.0;
          }

          // Apply similarity transformation to remaining columns.

          for (int j = 0; j < i; j++)
          {
            f = d[j];
            v[j][i] = f;
            g = e[j] + v[j][j] * f;
            for (int k = j + 1; k <= i - 1; k++)
            {
              g += v[k][j] * d[k];
              e[k] += v[k][j] * f;
            }
            e[j] = g;
          }
          f = 0.0;
          for (int j = 0; j < i; j++)
          {
            e[j] /= h;
            f += e[j] * d[j];
          }
          double hh = f / (h + h);
          for (int j = 0; j < i; j++)
          {
            e[j] -= hh * d[j];
          }
          for (int j = 0; j < i; j++)
          {
            f = d[j];
            g = e[j];
            for (int k = j; k <= i - 1; k++)
            {
              v[k][j] -= (f * e[k] + g * d[k]);
            }
            d[j] = v[i - 1][j];
            v[i][j] = 0.0;
          }
        }
        d[i] = h;
      }

      // Accumulate transformations.

      for (int i = 0; i < n - 1; i++)
      {
        v[n - 1][i] = v[i][i];
        v[i][i] = 1.0;
        double h = d[i + 1];
        if (h != 0.0)
        {
          for (int k = 0; k <= i; k++)
          {
            d[k] = v[k][i + 1] / h;
          }
          for (int j = 0; j <= i; j++)
          {
            double g = 0.0;
            for (int k = 0; k <= i; k++)
            {
              g += v[k][i + 1] * v[k][j];
            }
            for (int k = 0; k <= i; k++)
            {
              v[k][j] -= g * d[k];
            }
          }
        }
        for (int k = 0; k <= i; k++)
        {
          v[k][i + 1] = 0.0;
        }
      }
      for (int j = 0; j < n; j++)
      {
        d[j] = v[n - 1][j];
        v[n - 1][j] = 0.0;
      }
      v[n - 1][n - 1] = 1.0;
      e[0] = 0.0;
    }

    // Symmetric tridiagonal QL algorithm.

    private void tql2()
    {
      //  This is derived from the Algol procedures tql2, by
      //  Bowdler, Martin, Reinsch, and Wilkinson, Handbook for
      //  Auto. Comp., Vol.ii-Linear Algebra, and the corresponding
      //  Fortran subroutine in EISPACK.

      for (int i = 1; i < n; i++)
      {
        e[i - 1] = e[i];
      }
      e[n - 1] = 0.0;

      double f = 0.0;
      double tst1 = 0.0;
      double eps = System.Math.Pow(2.0, -52.0);
      for (int l = 0; l < n; l++)
      {
        // Find small subdiagonal element

        tst1 = System.Math.Max(tst1, System.Math.Abs(d[l]) + System.Math.Abs(e[l]));
        int m = l;
        while (m < n)
        {
          if (System.Math.Abs(e[m]) <= eps * tst1)
          {
            break;
          }
          m++;
        }

        // If m == l, d[l] is an eigenvalue,
        // otherwise, iterate.

        if (m > l)
        {
          int iter = 0;
          do
          {
            iter = iter + 1; // (Could check iteration count here.)

            // Compute implicit shift

            double g = d[l];
            double p = (d[l + 1] - g) / (2.0 * e[l]);
            double r = Maths.Hypot(p, 1.0);
            if (p < 0)
            {
              r = -r;
            }
            d[l] = e[l] / (p + r);
            d[l + 1] = e[l] * (p + r);
            double dl1 = d[l + 1];
            double h = g - d[l];
            for (int i = l + 2; i < n; i++)
            {
              d[i] -= h;
            }
            f = f + h;

            // Implicit QL transformation.

            p = d[m];
            double c = 1.0;
            double c2 = c;
            double c3 = c;
            double el1 = e[l + 1];
            double s = 0.0;
            double s2 = 0.0;
            for (int i = m - 1; i >= l; i--)
            {
              c3 = c2;
              c2 = c;
              s2 = s;
              g = c * e[i];
              h = c * p;
              r = Maths.Hypot(p, e[i]);
              e[i + 1] = s * r;
              s = e[i] / r;
              c = p / r;
              p = c * d[i] - s * g;
              d[i + 1] = h + s * (c * g + s * d[i]);

              // Accumulate transformation.

              for (int k = 0; k < n; k++)
              {
                h = v[k][i + 1];
                v[k][i + 1] = s * v[k][i] + c * h;
                v[k][i] = c * v[k][i] - s * h;
              }
            }
            p = (-s) * s2 * c3 * el1 * e[l] / dl1;
            e[l] = s * p;
            d[l] = c * p;

            // Check for convergence.
          }
          while (System.Math.Abs(e[l]) > eps * tst1);
        }
        d[l] = d[l] + f;
        e[l] = 0.0;
      }

      // Sort eigenvalues and corresponding vectors.

      for (int i = 0; i < n - 1; i++)
      {
        int k = i;
        double p = d[i];
        for (int j = i + 1; j < n; j++)
        {
          if (d[j] < p)
          {
            k = j;
            p = d[j];
          }
        }
        if (k != i)
        {
          d[k] = d[i];
          d[i] = p;
          for (int j = 0; j < n; j++)
          {
            p = v[j][i];
            v[j][i] = v[j][k];
            v[j][k] = p;
          }
        }
      }
    }

    // Nonsymmetric reduction to Hessenberg form.

    private void orthes()
    {
      //  This is derived from the Algol procedures orthes and ortran,
      //  by Martin and Wilkinson, Handbook for Auto. Comp.,
      //  Vol.ii-Linear Algebra, and the corresponding
      //  Fortran subroutines in EISPACK.

      int low = 0;
      int high = n - 1;

      for (int m = low + 1; m <= high - 1; m++)
      {

        // Scale column.

        double scale = 0.0;
        for (int i = m; i <= high; i++)
        {
          scale = scale + System.Math.Abs(H[i][m - 1]);
        }
        if (scale != 0.0)
        {

          // Compute Householder transformation.

          double h = 0.0;
          for (int i = high; i >= m; i--)
          {
            ort[i] = H[i][m - 1] / scale;
            h += ort[i] * ort[i];
          }
          double g = System.Math.Sqrt(h);
          if (ort[m] > 0)
          {
            g = -g;
          }
          h = h - ort[m] * g;
          ort[m] = ort[m] - g;

          // Apply Householder similarity transformation
          // H = (I-u*u'/h)*H*(I-u*u')/h)

          for (int j = m; j < n; j++)
          {
            double f = 0.0;
            for (int i = high; i >= m; i--)
            {
              f += ort[i] * H[i][j];
            }
            f = f / h;
            for (int i = m; i <= high; i++)
            {
              H[i][j] -= f * ort[i];
            }
          }

          for (int i = 0; i <= high; i++)
          {
            double f = 0.0;
            for (int j = high; j >= m; j--)
            {
              f += ort[j] * H[i][j];
            }
            f = f / h;
            for (int j = m; j <= high; j++)
            {
              H[i][j] -= f * ort[j];
            }
          }
          ort[m] = scale * ort[m];
          H[m][m - 1] = scale * g;
        }
      }

      // Accumulate transformations (Algol's ortran).

      for (int i = 0; i < n; i++)
      {
        for (int j = 0; j < n; j++)
        {
          v[i][j] = (i == j ? 1.0 : 0.0);
        }
      }

      for (int m = high - 1; m >= low + 1; m--)
      {
        if (H[m][m - 1] != 0.0)
        {
          for (int i = m + 1; i <= high; i++)
          {
            ort[i] = H[i][m - 1];
          }
          for (int j = m; j <= high; j++)
          {
            double g = 0.0;
            for (int i = m; i <= high; i++)
            {
              g += ort[i] * v[i][j];
            }
            // Double division avoids possible underflow
            g = (g / ort[m]) / H[m][m - 1];
            for (int i = m; i <= high; i++)
            {
              v[i][j] += g * ort[i];
            }
          }
        }
      }
    }


    // Complex scalar division.

    [NonSerialized()]
    private double cdivr, cdivi;

    private void cdiv(double xr, double xi, double yr, double yi)
    {
      double r, d;
      if (System.Math.Abs(yr) > System.Math.Abs(yi))
      {
        r = yi / yr;
        d = yr + r * yi;
        cdivr = (xr + r * xi) / d;
        cdivi = (xi - r * xr) / d;
      }
      else
      {
        r = yr / yi;
        d = yi + r * yr;
        cdivr = (r * xr + xi) / d;
        cdivi = (r * xi - xr) / d;
      }
    }


    // Nonsymmetric reduction from Hessenberg to real Schur form.

    private void hqr2()
    {
      //  This is derived from the Algol procedure hqr2,
      //  by Martin and Wilkinson, Handbook for Auto. Comp.,
      //  Vol.ii-Linear Algebra, and the corresponding
      //  Fortran subroutine in EISPACK.

      // Initialize

      int nn = this.n;
      int n = nn - 1;
      int low = 0;
      int high = nn - 1;
      double eps = System.Math.Pow(2.0, -52.0);
      double exshift = 0.0;
      double p = 0, q = 0, r = 0, s = 0, z = 0, t, w, x, y;

      // Store roots isolated by balanc and compute matrix norm

      double norm = 0.0;
      for (int i = 0; i < nn; i++)
      {
        if (i < low | i > high)
        {
          d[i] = H[i][i];
          e[i] = 0.0;
        }
        for (int j = System.Math.Max(i - 1, 0); j < nn; j++)
        {
          norm = norm + System.Math.Abs(H[i][j]);
        }
      }

      // Outer loop over eigenvalue index

      int iter = 0;
      while (n >= low)
      {

        // Look for single small sub-diagonal element

        int l = n;
        while (l > low)
        {
          s = System.Math.Abs(H[l - 1][l - 1]) + System.Math.Abs(H[l][l]);
          if (s == 0.0)
          {
            s = norm;
          }
          if (System.Math.Abs(H[l][l - 1]) < eps * s)
          {
            break;
          }
          l--;
        }

        // Check for convergence
        // One root found

        if (l == n)
        {
          H[n][n] = H[n][n] + exshift;
          d[n] = H[n][n];
          e[n] = 0.0;
          n--;
          iter = 0;

          // Two roots found
        }
        else if (l == n - 1)
        {
          w = H[n][n - 1] * H[n - 1][n];
          p = (H[n - 1][n - 1] - H[n][n]) / 2.0;
          q = p * p + w;
          z = System.Math.Sqrt(System.Math.Abs(q));
          H[n][n] = H[n][n] + exshift;
          H[n - 1][n - 1] = H[n - 1][n - 1] + exshift;
          x = H[n][n];

          // Real pair

          if (q >= 0)
          {
            if (p >= 0)
            {
              z = p + z;
            }
            else
            {
              z = p - z;
            }
            d[n - 1] = x + z;
            d[n] = d[n - 1];
            if (z != 0.0)
            {
              d[n] = x - w / z;
            }
            e[n - 1] = 0.0;
            e[n] = 0.0;
            x = H[n][n - 1];
            s = System.Math.Abs(x) + System.Math.Abs(z);
            p = x / s;
            q = z / s;
            r = System.Math.Sqrt(p * p + q * q);
            p = p / r;
            q = q / r;

            // Row modification

            for (int j = n - 1; j < nn; j++)
            {
              z = H[n - 1][j];
              H[n - 1][j] = q * z + p * H[n][j];
              H[n][j] = q * H[n][j] - p * z;
            }

            // Column modification

            for (int i = 0; i <= n; i++)
            {
              z = H[i][n - 1];
              H[i][n - 1] = q * z + p * H[i][n];
              H[i][n] = q * H[i][n] - p * z;
            }

            // Accumulate transformations

            for (int i = low; i <= high; i++)
            {
              z = v[i][n - 1];
              v[i][n - 1] = q * z + p * v[i][n];
              v[i][n] = q * v[i][n] - p * z;
            }

            // Complex pair
          }
          else
          {
            d[n - 1] = x + p;
            d[n] = x + p;
            e[n - 1] = z;
            e[n] = -z;
          }
          n = n - 2;
          iter = 0;

          // No convergence yet
        }
        else
        {

          // Form shift

          x = H[n][n];
          y = 0.0;
          w = 0.0;
          if (l < n)
          {
            y = H[n - 1][n - 1];
            w = H[n][n - 1] * H[n - 1][n];
          }

          // Wilkinson's original ad hoc shift

          if (iter == 10)
          {
            exshift += x;
            for (int i = low; i <= n; i++)
            {
              H[i][i] -= x;
            }
            s = System.Math.Abs(H[n][n - 1]) + System.Math.Abs(H[n - 1][n - 2]);
            x = y = 0.75 * s;
            w = (-0.4375) * s * s;
          }

          // MATLAB's new ad hoc shift

          if (iter == 30)
          {
            s = (y - x) / 2.0;
            s = s * s + w;
            if (s > 0)
            {
              s = System.Math.Sqrt(s);
              if (y < x)
              {
                s = -s;
              }
              s = x - w / ((y - x) / 2.0 + s);
              for (int i = low; i <= n; i++)
              {
                H[i][i] -= s;
              }
              exshift += s;
              x = y = w = 0.964;
            }
          }

          iter = iter + 1; // (Could check iteration count here.)

          // Look for two consecutive small sub-diagonal elements

          int m = n - 2;
          while (m >= l)
          {
            z = H[m][m];
            r = x - z;
            s = y - z;
            p = (r * s - w) / H[m + 1][m] + H[m][m + 1];
            q = H[m + 1][m + 1] - z - r - s;
            r = H[m + 2][m + 1];
            s = System.Math.Abs(p) + System.Math.Abs(q) + System.Math.Abs(r);
            p = p / s;
            q = q / s;
            r = r / s;
            if (m == l)
            {
              break;
            }
            if (System.Math.Abs(H[m][m - 1]) * (System.Math.Abs(q) + System.Math.Abs(r)) < eps * (System.Math.Abs(p) * (System.Math.Abs(H[m - 1][m - 1]) + System.Math.Abs(z) + System.Math.Abs(H[m + 1][m + 1]))))
            {
              break;
            }
            m--;
          }

          for (int i = m + 2; i <= n; i++)
          {
            H[i][i - 2] = 0.0;
            if (i > m + 2)
            {
              H[i][i - 3] = 0.0;
            }
          }

          // Double QR step involving rows l:n and columns m:n

          for (int k = m; k <= n - 1; k++)
          {
            bool notlast = (k != n - 1);
            if (k != m)
            {
              p = H[k][k - 1];
              q = H[k + 1][k - 1];
              r = (notlast ? H[k + 2][k - 1] : 0.0);
              x = System.Math.Abs(p) + System.Math.Abs(q) + System.Math.Abs(r);
              if (x != 0.0)
              {
                p = p / x;
                q = q / x;
                r = r / x;
              }
            }
            if (x == 0.0)
            {
              break;
            }
            s = System.Math.Sqrt(p * p + q * q + r * r);
            if (p < 0)
            {
              s = -s;
            }
            if (s != 0)
            {
              if (k != m)
              {
                H[k][k - 1] = (-s) * x;
              }
              else if (l != m)
              {
                H[k][k - 1] = -H[k][k - 1];
              }
              p = p + s;
              x = p / s;
              y = q / s;
              z = r / s;
              q = q / p;
              r = r / p;

              // Row modification

              for (int j = k; j < nn; j++)
              {
                p = H[k][j] + q * H[k + 1][j];
                if (notlast)
                {
                  p = p + r * H[k + 2][j];
                  H[k + 2][j] = H[k + 2][j] - p * z;
                }
                H[k][j] = H[k][j] - p * x;
                H[k + 1][j] = H[k + 1][j] - p * y;
              }

              // Column modification

              for (int i = 0; i <= System.Math.Min(n, k + 3); i++)
              {
                p = x * H[i][k] + y * H[i][k + 1];
                if (notlast)
                {
                  p = p + z * H[i][k + 2];
                  H[i][k + 2] = H[i][k + 2] - p * r;
                }
                H[i][k] = H[i][k] - p;
                H[i][k + 1] = H[i][k + 1] - p * q;
              }

              // Accumulate transformations

              for (int i = low; i <= high; i++)
              {
                p = x * v[i][k] + y * v[i][k + 1];
                if (notlast)
                {
                  p = p + z * v[i][k + 2];
                  v[i][k + 2] = v[i][k + 2] - p * r;
                }
                v[i][k] = v[i][k] - p;
                v[i][k + 1] = v[i][k + 1] - p * q;
              }
            } // (s != 0)
          } // k loop
        } // check convergence
      } // while (n >= low)

      // Backsubstitute to find vectors of upper triangular form

      if (norm == 0.0)
      {
        return;
      }

      for (n = nn - 1; n >= 0; n--)
      {
        p = d[n];
        q = e[n];

        // Real vector

        if (q == 0)
        {
          int l = n;
          H[n][n] = 1.0;
          for (int i = n - 1; i >= 0; i--)
          {
            w = H[i][i] - p;
            r = 0.0;
            for (int j = l; j <= n; j++)
            {
              r = r + H[i][j] * H[j][n];
            }
            if (e[i] < 0.0)
            {
              z = w;
              s = r;
            }
            else
            {
              l = i;
              if (e[i] == 0.0)
              {
                if (w != 0.0)
                {
                  H[i][n] = (-r) / w;
                }
                else
                {
                  H[i][n] = (-r) / (eps * norm);
                }

                // Solve real equations
              }
              else
              {
                x = H[i][i + 1];
                y = H[i + 1][i];
                q = (d[i] - p) * (d[i] - p) + e[i] * e[i];
                t = (x * s - z * r) / q;
                H[i][n] = t;
                if (System.Math.Abs(x) > System.Math.Abs(z))
                {
                  H[i + 1][n] = (-r - w * t) / x;
                }
                else
                {
                  H[i + 1][n] = (-s - y * t) / z;
                }
              }

              // Overflow control

              t = System.Math.Abs(H[i][n]);
              if ((eps * t) * t > 1)
              {
                for (int j = i; j <= n; j++)
                {
                  H[j][n] = H[j][n] / t;
                }
              }
            }
          }

          // Complex vector
        }
        else if (q < 0)
        {
          int l = n - 1;

          // Last vector component imaginary so matrix is triangular

          if (System.Math.Abs(H[n][n - 1]) > System.Math.Abs(H[n - 1][n]))
          {
            H[n - 1][n - 1] = q / H[n][n - 1];
            H[n - 1][n] = (-(H[n][n] - p)) / H[n][n - 1];
          }
          else
          {
            cdiv(0.0, -H[n - 1][n], H[n - 1][n - 1] - p, q);
            H[n - 1][n - 1] = cdivr;
            H[n - 1][n] = cdivi;
          }
          H[n][n - 1] = 0.0;
          H[n][n] = 1.0;
          for (int i = n - 2; i >= 0; i--)
          {
            double ra, sa, vr, vi;
            ra = 0.0;
            sa = 0.0;
            for (int j = l; j <= n; j++)
            {
              ra = ra + H[i][j] * H[j][n - 1];
              sa = sa + H[i][j] * H[j][n];
            }
            w = H[i][i] - p;

            if (e[i] < 0.0)
            {
              z = w;
              r = ra;
              s = sa;
            }
            else
            {
              l = i;
              if (e[i] == 0)
              {
                cdiv(-ra, -sa, w, q);
                H[i][n - 1] = cdivr;
                H[i][n] = cdivi;
              }
              else
              {

                // Solve complex equations

                x = H[i][i + 1];
                y = H[i + 1][i];
                vr = (d[i] - p) * (d[i] - p) + e[i] * e[i] - q * q;
                vi = (d[i] - p) * 2.0 * q;
                if (vr == 0.0 & vi == 0.0)
                {
                  vr = eps * norm * (System.Math.Abs(w) + System.Math.Abs(q) + System.Math.Abs(x) + System.Math.Abs(y) + System.Math.Abs(z));
                }
                cdiv(x * r - z * ra + q * sa, x * s - z * sa - q * ra, vr, vi);
                H[i][n - 1] = cdivr;
                H[i][n] = cdivi;
                if (System.Math.Abs(x) > (System.Math.Abs(z) + System.Math.Abs(q)))
                {
                  H[i + 1][n - 1] = (-ra - w * H[i][n - 1] + q * H[i][n]) / x;
                  H[i + 1][n] = (-sa - w * H[i][n] - q * H[i][n - 1]) / x;
                }
                else
                {
                  cdiv(-r - y * H[i][n - 1], -s - y * H[i][n], z, q);
                  H[i + 1][n - 1] = cdivr;
                  H[i + 1][n] = cdivi;
                }
              }

              // Overflow control

              t = System.Math.Max(System.Math.Abs(H[i][n - 1]), System.Math.Abs(H[i][n]));
              if ((eps * t) * t > 1)
              {
                for (int j = i; j <= n; j++)
                {
                  H[j][n - 1] = H[j][n - 1] / t;
                  H[j][n] = H[j][n] / t;
                }
              }
            }
          }
        }
      }

      // Vectors of isolated roots

      for (int i = 0; i < nn; i++)
      {
        if (i < low | i > high)
        {
          for (int j = i; j < nn; j++)
          {
            v[i][j] = H[i][j];
          }
        }
      }

      // Back transformation to get eigenvectors of original matrix

      for (int j = nn - 1; j >= low; j--)
      {
        for (int i = low; i <= high; i++)
        {
          z = 0.0;
          for (int k = low; k <= System.Math.Min(j, high); k++)
          {
            z = z + v[i][k] * H[k][j];
          }
          v[i][j] = z;
        }
      }
    }

    #endregion //  Private Methods


    #region Constructor

    /// <summary>Check for symmetry, then construct the eigenvalue decomposition</summary>
    /// <param name="Arg">   Square matrix
    /// </param>
    /// <returns>     Structure to access D and V.
    /// </returns>

    public EigenvalueDecomposition(GeneralMatrix Arg)
    {
      double[][] A = Arg.Array;
      n = Arg.ColumnDimension;
      v = new double[n][];
      for (int i = 0; i < n; i++)
      {
        v[i] = new double[n];
      }
      d = new double[n];
      e = new double[n];

      issymmetric = true;
      for (int j = 0; (j < n) & issymmetric; j++)
      {
        for (int i = 0; (i < n) & issymmetric; i++)
        {
          issymmetric = (A[i][j] == A[j][i]);
        }
      }

      if (issymmetric)
      {
        for (int i = 0; i < n; i++)
        {
          for (int j = 0; j < n; j++)
          {
            v[i][j] = A[i][j];
          }
        }

        // Tridiagonalize.
        tred2();

        // Diagonalize.
        tql2();
      }
      else
      {
        H = new double[n][];
        for (int i2 = 0; i2 < n; i2++)
        {
          H[i2] = new double[n];
        }
        ort = new double[n];

        for (int j = 0; j < n; j++)
        {
          for (int i = 0; i < n; i++)
          {
            H[i][j] = A[i][j];
          }
        }

        // Reduce to Hessenberg form.
        orthes();

        // Reduce Hessenberg to real Schur form.
        hqr2();
      }
    }

    #endregion //  Constructor

    #region Public Properties
    /// <summary>Return the real parts of the eigenvalues</summary>
    /// <returns>     real(diag(D))
    /// </returns>
    virtual public double[] RealEigenvalues
    {
      get
      {
        return d;
      }
    }
    /// <summary>Return the imaginary parts of the eigenvalues</summary>
    /// <returns>     imag(diag(D))
    /// </returns>
    virtual public double[] ImagEigenvalues
    {
      get
      {
        return e;
      }
    }
    /// <summary>Return the block diagonal eigenvalue matrix</summary>
    /// <returns>     D
    /// </returns>
    virtual public GeneralMatrix D
    {
      get
      {
        GeneralMatrix X = new GeneralMatrix(n, n);
        double[][] D = X.Array;
        for (int i = 0; i < n; i++)
        {
          for (int j = 0; j < n; j++)
          {
            D[i][j] = 0.0;
          }
          D[i][i] = d[i];
          if (e[i] > 0)
          {
            D[i][i + 1] = e[i];
          }
          else if (e[i] < 0)
          {
            D[i][i - 1] = e[i];
          }
        }
        return X;
      }
    }
    #endregion //  Public Properties

    #region Public Methods

    /// <summary>Return the eigenvector matrix</summary>
    /// <returns>     V
    /// </returns>

    public virtual GeneralMatrix GetV()
    {
      return new GeneralMatrix(v, n, n);
    }
    #endregion //  Public Methods

    // A method called when serializing this class.
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
    }
  }

  /// <summary>Singular Value Decomposition.
  /// <P>
  /// For an m-by-n matrix A with m >= n, the singular value decomposition is
  /// an m-by-n orthogonal matrix U, an n-by-n diagonal matrix S, and
  /// an n-by-n orthogonal matrix V so that A = U*S*V'.
  /// <P>
  /// The singular values, sigma[k] = S[k][k], are ordered so that
  /// sigma[0] >= sigma[1] >= ... >= sigma[n-1].
  /// <P>
  /// The singular value decompostion always exists, so the constructor will
  /// never fail.  The matrix condition number and the effective numerical
  /// rank can be computed from this decomposition.
  /// </summary>

  [Serializable]
  public class SingularValueDecomposition : System.Runtime.Serialization.ISerializable
  {
    #region Class variables

    /// <summary>Arrays for internal storage of U and V.
    /// @serial internal storage of U.
    /// @serial internal storage of V.
    /// </summary>
    private double[][] u, v;

    /// <summary>Array for internal storage of singular values.
    /// @serial internal storage of singular values.
    /// </summary>
    private double[] s;

    /// <summary>Row and column dimensions.
    /// @serial row dimension.
    /// @serial column dimension.
    /// </summary>
    private int m, n;

    #endregion   //Class variables

    #region Constructor

    /// <summary>Construct the singular value decomposition</summary>
    /// <param name="Arg">   Rectangular matrix
    /// </param>
    /// <returns>     Structure to access U, S and V.
    /// </returns>

    public SingularValueDecomposition(GeneralMatrix Arg)
    {
      // Derived from LINPACK code.
      // Initialize.
      double[][] A = Arg.ArrayCopy;
      m = Arg.RowDimension;
      n = Arg.ColumnDimension;
      int nu = System.Math.Min(m, n);
      s = new double[System.Math.Min(m + 1, n)];
      u = new double[m][];
      for (int i = 0; i < m; i++)
      {
        u[i] = new double[nu];
      }
      v = new double[n][];
      for (int i2 = 0; i2 < n; i2++)
      {
        v[i2] = new double[n];
      }
      double[] e = new double[n];
      double[] work = new double[m];
      bool wantu = true;
      bool wantv = true;

      // Reduce A to bidiagonal form, storing the diagonal elements
      // in s and the super-diagonal elements in e.

      int nct = System.Math.Min(m - 1, n);
      int nrt = System.Math.Max(0, System.Math.Min(n - 2, m));
      for (int k = 0; k < System.Math.Max(nct, nrt); k++)
      {
        if (k < nct)
        {

          // Compute the transformation for the k-th column and
          // place the k-th diagonal in s[k].
          // Compute 2-norm of k-th column without under/overflow.
          s[k] = 0;
          for (int i = k; i < m; i++)
          {
            s[k] = Maths.Hypot(s[k], A[i][k]);
          }
          if (s[k] != 0.0)
          {
            if (A[k][k] < 0.0)
            {
              s[k] = -s[k];
            }
            for (int i = k; i < m; i++)
            {
              A[i][k] /= s[k];
            }
            A[k][k] += 1.0;
          }
          s[k] = -s[k];
        }
        for (int j = k + 1; j < n; j++)
        {
          if ((k < nct) & (s[k] != 0.0))
          {

            // Apply the transformation.

            double t = 0;
            for (int i = k; i < m; i++)
            {
              t += A[i][k] * A[i][j];
            }
            t = (-t) / A[k][k];
            for (int i = k; i < m; i++)
            {
              A[i][j] += t * A[i][k];
            }
          }

          // Place the k-th row of A into e for the
          // subsequent calculation of the row transformation.

          e[j] = A[k][j];
        }
        if (wantu & (k < nct))
        {

          // Place the transformation in U for subsequent back
          // multiplication.

          for (int i = k; i < m; i++)
          {
            u[i][k] = A[i][k];
          }
        }
        if (k < nrt)
        {

          // Compute the k-th row transformation and place the
          // k-th super-diagonal in e[k].
          // Compute 2-norm without under/overflow.
          e[k] = 0;
          for (int i = k + 1; i < n; i++)
          {
            e[k] = Maths.Hypot(e[k], e[i]);
          }
          if (e[k] != 0.0)
          {
            if (e[k + 1] < 0.0)
            {
              e[k] = -e[k];
            }
            for (int i = k + 1; i < n; i++)
            {
              e[i] /= e[k];
            }
            e[k + 1] += 1.0;
          }
          e[k] = -e[k];
          if ((k + 1 < m) & (e[k] != 0.0))
          {

            // Apply the transformation.

            for (int i = k + 1; i < m; i++)
            {
              work[i] = 0.0;
            }
            for (int j = k + 1; j < n; j++)
            {
              for (int i = k + 1; i < m; i++)
              {
                work[i] += e[j] * A[i][j];
              }
            }
            for (int j = k + 1; j < n; j++)
            {
              double t = (-e[j]) / e[k + 1];
              for (int i = k + 1; i < m; i++)
              {
                A[i][j] += t * work[i];
              }
            }
          }
          if (wantv)
          {

            // Place the transformation in V for subsequent
            // back multiplication.

            for (int i = k + 1; i < n; i++)
            {
              v[i][k] = e[i];
            }
          }
        }
      }

      // Set up the final bidiagonal matrix or order p.

      int p = System.Math.Min(n, m + 1);
      if (nct < n)
      {
        s[nct] = A[nct][nct];
      }
      if (m < p)
      {
        s[p - 1] = 0.0;
      }
      if (nrt + 1 < p)
      {
        e[nrt] = A[nrt][p - 1];
      }
      e[p - 1] = 0.0;

      // If required, generate U.

      if (wantu)
      {
        for (int j = nct; j < nu; j++)
        {
          for (int i = 0; i < m; i++)
          {
            u[i][j] = 0.0;
          }
          u[j][j] = 1.0;
        }
        for (int k = nct - 1; k >= 0; k--)
        {
          if (s[k] != 0.0)
          {
            for (int j = k + 1; j < nu; j++)
            {
              double t = 0;
              for (int i = k; i < m; i++)
              {
                t += u[i][k] * u[i][j];
              }
              t = (-t) / u[k][k];
              for (int i = k; i < m; i++)
              {
                u[i][j] += t * u[i][k];
              }
            }
            for (int i = k; i < m; i++)
            {
              u[i][k] = -u[i][k];
            }
            u[k][k] = 1.0 + u[k][k];
            for (int i = 0; i < k - 1; i++)
            {
              u[i][k] = 0.0;
            }
          }
          else
          {
            for (int i = 0; i < m; i++)
            {
              u[i][k] = 0.0;
            }
            u[k][k] = 1.0;
          }
        }
      }

      // If required, generate V.

      if (wantv)
      {
        for (int k = n - 1; k >= 0; k--)
        {
          if ((k < nrt) & (e[k] != 0.0))
          {
            for (int j = k + 1; j < nu; j++)
            {
              double t = 0;
              for (int i = k + 1; i < n; i++)
              {
                t += v[i][k] * v[i][j];
              }
              t = (-t) / v[k + 1][k];
              for (int i = k + 1; i < n; i++)
              {
                v[i][j] += t * v[i][k];
              }
            }
          }
          for (int i = 0; i < n; i++)
          {
            v[i][k] = 0.0;
          }
          v[k][k] = 1.0;
        }
      }

      // Main iteration loop for the singular values.

      int pp = p - 1;
      int iter = 0;
      double eps = System.Math.Pow(2.0, -52.0);
      while (p > 0)
      {
        int k, kase;

        // Here is where a test for too many iterations would go.

        // This section of the program inspects for
        // negligible elements in the s and e arrays.  On
        // completion the variables kase and k are set as follows.

        // kase = 1     if s(p) and e[k-1] are negligible and k<p
        // kase = 2     if s(k) is negligible and k<p
        // kase = 3     if e[k-1] is negligible, k<p, and
        //              s(k), ..., s(p) are not negligible (qr step).
        // kase = 4     if e(p-1) is negligible (convergence).

        for (k = p - 2; k >= -1; k--)
        {
          if (k == -1)
          {
            break;
          }
          if (System.Math.Abs(e[k]) <= eps * (System.Math.Abs(s[k]) + System.Math.Abs(s[k + 1])))
          {
            e[k] = 0.0;
            break;
          }
        }
        if (k == p - 2)
        {
          kase = 4;
        }
        else
        {
          int ks;
          for (ks = p - 1; ks >= k; ks--)
          {
            if (ks == k)
            {
              break;
            }
            double t = (ks != p ? System.Math.Abs(e[ks]) : 0.0) + (ks != k + 1 ? System.Math.Abs(e[ks - 1]) : 0.0);
            if (System.Math.Abs(s[ks]) <= eps * t)
            {
              s[ks] = 0.0;
              break;
            }
          }
          if (ks == k)
          {
            kase = 3;
          }
          else if (ks == p - 1)
          {
            kase = 1;
          }
          else
          {
            kase = 2;
            k = ks;
          }
        }
        k++;

        // Perform the task indicated by kase.

        switch (kase)
        {


          // Deflate negligible s(p).
          case 1:
            {
              double f = e[p - 2];
              e[p - 2] = 0.0;
              for (int j = p - 2; j >= k; j--)
              {
                double t = Maths.Hypot(s[j], f);
                double cs = s[j] / t;
                double sn = f / t;
                s[j] = t;
                if (j != k)
                {
                  f = (-sn) * e[j - 1];
                  e[j - 1] = cs * e[j - 1];
                }
                if (wantv)
                {
                  for (int i = 0; i < n; i++)
                  {
                    t = cs * v[i][j] + sn * v[i][p - 1];
                    v[i][p - 1] = (-sn) * v[i][j] + cs * v[i][p - 1];
                    v[i][j] = t;
                  }
                }
              }
            }
            break;

          // Split at negligible s(k).


          case 2:
            {
              double f = e[k - 1];
              e[k - 1] = 0.0;
              for (int j = k; j < p; j++)
              {
                double t = Maths.Hypot(s[j], f);
                double cs = s[j] / t;
                double sn = f / t;
                s[j] = t;
                f = (-sn) * e[j];
                e[j] = cs * e[j];
                if (wantu)
                {
                  for (int i = 0; i < m; i++)
                  {
                    t = cs * u[i][j] + sn * u[i][k - 1];
                    u[i][k - 1] = (-sn) * u[i][j] + cs * u[i][k - 1];
                    u[i][j] = t;
                  }
                }
              }
            }
            break;

          // Perform one qr step.


          case 3:
            {
              // Calculate the shift.

              double scale = System.Math.Max(System.Math.Max(System.Math.Max(System.Math.Max(System.Math.Abs(s[p - 1]), System.Math.Abs(s[p - 2])), System.Math.Abs(e[p - 2])), System.Math.Abs(s[k])), System.Math.Abs(e[k]));
              double sp = s[p - 1] / scale;
              double spm1 = s[p - 2] / scale;
              double epm1 = e[p - 2] / scale;
              double sk = s[k] / scale;
              double ek = e[k] / scale;
              double b = ((spm1 + sp) * (spm1 - sp) + epm1 * epm1) / 2.0;
              double c = (sp * epm1) * (sp * epm1);
              double shift = 0.0;
              if ((b != 0.0) | (c != 0.0))
              {
                shift = System.Math.Sqrt(b * b + c);
                if (b < 0.0)
                {
                  shift = -shift;
                }
                shift = c / (b + shift);
              }
              double f = (sk + sp) * (sk - sp) + shift;
              double g = sk * ek;

              // Chase zeros.

              for (int j = k; j < p - 1; j++)
              {
                double t = Maths.Hypot(f, g);
                double cs = f / t;
                double sn = g / t;
                if (j != k)
                {
                  e[j - 1] = t;
                }
                f = cs * s[j] + sn * e[j];
                e[j] = cs * e[j] - sn * s[j];
                g = sn * s[j + 1];
                s[j + 1] = cs * s[j + 1];
                if (wantv)
                {
                  for (int i = 0; i < n; i++)
                  {
                    t = cs * v[i][j] + sn * v[i][j + 1];
                    v[i][j + 1] = (-sn) * v[i][j] + cs * v[i][j + 1];
                    v[i][j] = t;
                  }
                }
                t = Maths.Hypot(f, g);
                cs = f / t;
                sn = g / t;
                s[j] = t;
                f = cs * e[j] + sn * s[j + 1];
                s[j + 1] = (-sn) * e[j] + cs * s[j + 1];
                g = sn * e[j + 1];
                e[j + 1] = cs * e[j + 1];
                if (wantu && (j < m - 1))
                {
                  for (int i = 0; i < m; i++)
                  {
                    t = cs * u[i][j] + sn * u[i][j + 1];
                    u[i][j + 1] = (-sn) * u[i][j] + cs * u[i][j + 1];
                    u[i][j] = t;
                  }
                }
              }
              e[p - 2] = f;
              iter = iter + 1;
            }
            break;

          // Convergence.


          case 4:
            {
              // Make the singular values positive.

              if (s[k] <= 0.0)
              {
                s[k] = (s[k] < 0.0 ? -s[k] : 0.0);
                if (wantv)
                {
                  for (int i = 0; i <= pp; i++)
                  {
                    v[i][k] = -v[i][k];
                  }
                }
              }

              // Order the singular values.

              while (k < pp)
              {
                if (s[k] >= s[k + 1])
                {
                  break;
                }
                double t = s[k];
                s[k] = s[k + 1];
                s[k + 1] = t;
                if (wantv && (k < n - 1))
                {
                  for (int i = 0; i < n; i++)
                  {
                    t = v[i][k + 1]; v[i][k + 1] = v[i][k]; v[i][k] = t;
                  }
                }
                if (wantu && (k < m - 1))
                {
                  for (int i = 0; i < m; i++)
                  {
                    t = u[i][k + 1]; u[i][k + 1] = u[i][k]; u[i][k] = t;
                  }
                }
                k++;
              }
              iter = 0;
              p--;
            }
            break;
        }
      }
    }
    #endregion	//Constructor

    #region Public Properties
    /// <summary>Return the one-dimensional array of singular values</summary>
    /// <returns>     diagonal of S.
    /// </returns>
    virtual public double[] SingularValues
    {
      get
      {
        return s;
      }
    }

    /// <summary>Return the diagonal matrix of singular values</summary>
    /// <returns>     S
    /// </returns>
    virtual public GeneralMatrix S
    {
      get
      {
        GeneralMatrix X = new GeneralMatrix(n, n);
        double[][] S = X.Array;
        for (int i = 0; i < n; i++)
        {
          for (int j = 0; j < n; j++)
          {
            S[i][j] = 0.0;
          }
          S[i][i] = this.s[i];
        }
        return X;
      }
    }
    #endregion //  Public Properties

    #region	 Public Methods

    /// <summary>Return the left singular vectors</summary>
    /// <returns>     U
    /// </returns>

    public virtual GeneralMatrix GetU()
    {
      return new GeneralMatrix(u, m, System.Math.Min(m + 1, n));
    }

    /// <summary>Return the right singular vectors</summary>
    /// <returns>     V
    /// </returns>

    public virtual GeneralMatrix GetV()
    {
      return new GeneralMatrix(v, n, n);
    }

    /// <summary>Two norm</summary>
    /// <returns>     max(S)
    /// </returns>

    public virtual double Norm2()
    {
      return s[0];
    }

    /// <summary>Two norm condition number</summary>
    /// <returns>     max(S)/min(S)
    /// </returns>

    public virtual double Condition()
    {
      return s[0] / s[System.Math.Min(m, n) - 1];
    }

    /// <summary>Effective numerical matrix rank</summary>
    /// <returns>     Number of nonnegligible singular values.
    /// </returns>

    public virtual int Rank()
    {
      double eps = System.Math.Pow(2.0, -52.0);
      double tol = System.Math.Max(m, n) * s[0] * eps;
      int r = 0;
      for (int i = 0; i < s.Length; i++)
      {
        if (s[i] > tol)
        {
          r++;
        }
      }
      return r;
    }
    #endregion   //Public Methods

    // A method called when serializing this class.
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
    }
  }
}