using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using GBMSAPI_NET;
using GBMSAPI_NET.GBMSAPI_NET_Defines;
using GBMSAPI_NET.GBMSAPI_NET_Defines.GBMSAPI_NET_AcquisitionProcessDefines;
using GBMSAPI_NET.GBMSAPI_NET_Defines.GBMSAPI_NET_DeviceCharacteristicsDefines;
using GBMSAPI_NET.GBMSAPI_NET_Defines.GBMSAPI_NET_ErrorCodesDefines;
using GBMSAPI_NET.GBMSAPI_NET_Defines.GBMSAPI_NET_RollFunctionalityDefines;
using GBMSAPI_NET.GBMSAPI_NET_Defines.GBMSAPI_NET_VisualInterfaceLCDDefines;
using GBMSAPI_NET.GBMSAPI_NET_Defines.GBMSAPI_NET_VisualInterfaceLEDsDefines;
using GBMSAPI_NET.GBMSAPI_NET_LibraryFunctions;

using GBMSAPI_CS_Example.UTILITY;
using GBMSAPI_CS_Example.CALIBRATION;
using GBMSAPI_CS_Example.MAIN_FORM;
using GBMSAPI_CS_Example.ACQUISITION_FORM;

namespace GBMSAPI_CS_Example.PERMANENT_DATA_STORAGE
{
    public partial class PermanentDataStorageForm : Form
    {
        public int UserDataSize;
        public PermanentDataStorageForm()
        {
            InitializeComponent();

            int RetVal, Appoggio;
			RetVal =  GBMSAPI_NET_AuxiliaryRoutines.GBMSAPI_NET_GetUserDataSize(out Appoggio);

            if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
            {
                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal, "GBMSAPI_NET_GetUserDataSize");  

                this.Close();
            }
			else
			{
				UserDataSize = Appoggio;
				this.StorageSizeLabel.Text = "Available bytes = " + UserDataSize;
			}
        }

        private void WriteButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog  OFDlg = new OpenFileDialog();
			OFDlg.Title = "Open file containing data";
			if (OFDlg.ShowDialog() != DialogResult.OK )
			{
				MessageBox.Show("Error in Opening File");
				return;
			}
			String fname = OFDlg.FileName;
			Byte[] DataToBeWrittenArray = File.ReadAllBytes(fname);

			if (DataToBeWrittenArray.Length > (UserDataSize - 1)) // I need at least a byte at the end of the buffer for CRC calculation
			{
				MessageBox.Show("Warning: Data To be written too much long; they will be truncated to the " + UserDataSize + "-th byte");
			}

			int BytesToBeWritten;

			if (DataToBeWrittenArray.Length > (UserDataSize - 1))
			{
				BytesToBeWritten = (UserDataSize - 1);
			}
			else
			{
				BytesToBeWritten = DataToBeWrittenArray.Length;
			}

            int RetVal = GBMSAPI_NET_AuxiliaryRoutines.GBMSAPI_NET_WriteUserData(
                (uint)0, (uint)BytesToBeWritten, DataToBeWrittenArray
            );

            if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
			{
				GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
					RetVal,"PermanentDataStorageForm,GBMSAPI_WriteUserData");
			}
			else
			{
				MessageBox.Show("Written " + BytesToBeWritten + "bytes");
			}
        }

        private void ReadButton_Click(object sender, EventArgs e)
        {
            IntPtr BufferToRead = Marshal.AllocHGlobal(UserDataSize);
			if (BufferToRead == IntPtr.Zero)
			{
				MessageBox.Show("Error in Buffer allocation","ReadButton_Click");
				return;
			}

			Byte[] DataToBeWrittenArray = new Byte[UserDataSize];	
	
            int RetVal = GBMSAPI_NET_AuxiliaryRoutines.GBMSAPI_NET_ReadUserData(
                (uint)0, (uint)DataToBeWrittenArray.Length, DataToBeWrittenArray
            );

            if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
			{
				GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
					RetVal,"PermanentDataStorageForm,GBMSAPI_NET_ReadUserData");
			}

			SaveFileDialog  SFDlg = new SaveFileDialog();
			SFDlg.Title = "Open file for saving data";
			if (SFDlg.ShowDialog() != DialogResult.OK )
			{
				MessageBox.Show("Error in Opening File");
				return;
			}
			String fname = SFDlg.FileName;

			File.WriteAllBytes(fname,DataToBeWrittenArray);
        }
    }
}
