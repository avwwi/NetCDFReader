using NetCDFInterop;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int NC_MAX_NAME = 256;
        public const int NC_MAX_DIMS = 1024;
        public const int NC_FLOAT = 5;
        public const int NC_DOUBLE = 6;
        public const int NC_INT = 4;
        public const int NC_CHAR = 2;

        public MainWindow()
        {
            InitializeComponent();

            string fileName = @"C:\Users\Abhijat\Desktop\CDF\a0806_142.cdf";
            var ncID = 0;
            var openCDF = NetCDF.nc_open(fileName, CreateMode.NC_NETCDF4, out ncID);

            // Get file information
            if (NetCDF.nc_inq(ncID, out int ndims, out int nvars, out int natts, out int unlimdimid) == 0)
            {
                Debug.WriteLine($"Number of dimensions: {ndims}");
                Debug.WriteLine($"Number of variables: {nvars}");
                Debug.WriteLine($"Number of global attributes: {natts}");
                Debug.WriteLine($"Unlimited dimension ID: {unlimdimid}");
            }

            Debug.WriteLine("\nVariables:");

            // Iterate through variables
            for (int varid = 0; varid < nvars; varid++)
            {
                int[] dimids = new int[NC_MAX_DIMS];
                //StringBuilder varName = new StringBuilder(NC_MAX_NAME);
                string varName = null;

                // Get variable metadata
                if (NetCDF.nc_inq_var(ncID, varid, out varName, out NcType varType, out int ndimsVar, dimids, out int nattsVar) == 0)
                {
                    Debug.WriteLine($"Variable {varid}: Name = {varName}, Type = {varType}, Dimensions = {ndimsVar}, Attributes = {nattsVar}");

                    // Fetch and print variable data based on type
                    switch (varType)
                    {
                        case NcType.NC_FLOAT:
                            FetchAndPrintVariableDataFloat(ncID, varid, varName.ToString(), ndimsVar, dimids);
                            break;

                        case NcType.NC_DOUBLE:
                            FetchAndPrintVariableDataDouble(ncID, varid, varName.ToString(), ndimsVar, dimids);
                            break;

                        case NcType.NC_INT:
                            FetchAndPrintVariableDataInt(ncID, varid, varName.ToString(), ndimsVar, dimids);
                            break;

                        case NcType.NC_CHAR:
                            FetchAndPrintVariableDataChar(ncID, varid, varName.ToString(), ndimsVar, dimids);
                            break;

                        default:
                            Debug.WriteLine($"Unsupported variable type for {varName}");
                            break;
                    }
                }
                else
                {
                    Debug.WriteLine($"Error retrieving metadata for variable ID {varid}");
                }
            }

            // Close the NetCDF file
            if (NetCDF.nc_close(ncID) == 0)
            {
                Debug.WriteLine("File successfully closed.");
            }
            else
            {
                Debug.WriteLine("Error closing the file.");
            }
        }

        /// <summary>
        /// Fetch and print data for float variables.
        /// </summary>
        private void FetchAndPrintVariableDataFloat(int ncid, int varid, string varName, int ndims, int[] dimids)
        {
            nint[] start = new nint[ndims];
            nint[] count = GetDimensionSizes(ncid, dimids, ndims);

            if (count == null) return;

            float[] data = new float[CalculateTotalElements(count)];

            if (NetCDF.nc_get_vara_float(ncid, varid, start, count, data) == 0)
            {
                Debug.WriteLine($"Data for {varName}: {string.Join(", ", data)}");
            }
            else
            {
                Debug.WriteLine($"Error fetching data for variable {varName}");
            }
        }

        /// <summary>
        /// Fetch and print data for double variables.
        /// </summary>
        private void FetchAndPrintVariableDataDouble(int ncid, int varid, string varName, int ndims, int[] dimids)
        {
            nint[] start = new nint[ndims];
            nint[] count = GetDimensionSizes(ncid, dimids, ndims);

            if (count == null) return;

            double[] data = new double[CalculateTotalElements(count)];

            if (NetCDF.nc_get_vara_double(ncid, varid, start, count, data) == 0)
            {
                Debug.WriteLine($"Data for {varName}: {string.Join(", ", data.ToString())}");
            }
            else
            {
                Debug.WriteLine($"Error fetching data for variable {varName}");
            }
        }

        /// <summary>
        /// Fetch and print data for int variables.
        /// </summary>
        private void FetchAndPrintVariableDataInt(int ncid, int varid, string varName, int ndims, int[] dimids)
        {
            nint[] start = new nint[ndims];
            nint[] count = GetDimensionSizes(ncid, dimids, ndims);

            if (count == null) return;

            int[] data = new int[CalculateTotalElements(count)];

            if (NetCDF.nc_get_vara_int(ncid, varid, start, count, data) == 0)
            {
                Debug.WriteLine($"Data for {varName}: {string.Join(", ", data)}");
            }
            else
            {
                Debug.WriteLine($"Error fetching data for variable {varName}");
            }
        }

        /// <summary>
        /// Fetch and print data for char variables.
        /// </summary>
        private void FetchAndPrintVariableDataChar(int ncid, int varid, string varName, int ndims, int[] dimids)
        {
            nint[] start = new nint[ndims];
            nint[] count = GetDimensionSizes(ncid, dimids, ndims);

            if (count == null) return;

            byte[] data = new byte[CalculateTotalElements(count)];

            if (NetCDF.nc_get_vara_text(ncid, varid, start, count, data) == 0)
            {
                _ = data;
                Debug.WriteLine($"Data for {varName}: {data}");
                //Console.WriteLine($"Data for {varName}: {new string(data)}");
            }
            else
            {
                Debug.WriteLine($"Error fetching data for variable {varName}");
            }
        }

        /// <summary>
        /// Get dimension sizes.
        /// </summary>
        private nint[] GetDimensionSizes(int ncid, int[] dimids, int ndims)
        {
            nint[] sizes = new nint[ndims];

            for (int i = 0; i < ndims; i++)
            {
                if (NetCDF.nc_inq_dimlen(ncid, dimids[i], out sizes[i]) != 0)
                {
                    Debug.WriteLine($"Error retrieving dimension size for dimension ID {dimids[i]}");
                    return null;
                }
            }

            return sizes;
        }

        /// <summary>
        /// Calculate total number of elements.
        /// </summary>
        private long CalculateTotalElements(nint[] sizes)
        {
            long total = 1;

            foreach (var size in sizes)
            {
                total *= size;
            }

            return total;
        }
    }
}
