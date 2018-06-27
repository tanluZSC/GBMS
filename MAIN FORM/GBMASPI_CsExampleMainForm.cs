using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
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
using GBMSAPI_CS_Example.PERMANENT_DATA_STORAGE;

namespace GBMSAPI_CS_Example
{
    public partial class GBMASPI_CsExampleMainForm : Form
    {
        public GBMASPI_CsExampleMainForm()
        {
            InitializeComponent();
        }

        private void UpdateListButton_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            try
            {
                this.DeviceTypeComboBox.Items.Clear();
                ////////////////////////////////
                ///// Get device list
                ////////////////////////////////
                GBMSAPI_NET_DeviceInfoStruct[] AttachedDeviceList = new GBMSAPI_NET_DeviceInfoStruct[
                    GBMSAPI_NET_DeviceInfoConstants.GBMSAPI_NET_MAX_PLUGGED_DEVICE_NUM];
                for (int i = 0; i < GBMSAPI_NET_DeviceInfoConstants.GBMSAPI_NET_MAX_PLUGGED_DEVICE_NUM; i++)
                {
                    AttachedDeviceList[i] = new GBMSAPI_NET_DeviceInfoStruct();
                }

                int AttachedDeviceNumber;
                uint USBErrorCode;

                int RetVal = GBMSAPI_NET_DeviceSettingRoutines.GBMSAPI_NET_GetAttachedDeviceList(
                    AttachedDeviceList, out AttachedDeviceNumber, out USBErrorCode);
                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR || AttachedDeviceNumber <= 0)
                {
                    if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                    {
                        GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal, USBErrorCode, "UpdateListButton_Click");
                        MessageBox.Show("Error in GBMSAPI_NET_GetAttachedDeviceList function: " + RetVal);
                    }
                    return;
                }

                ////////////////////////////////
                //// Store device list
                ////////////////////////////////
                for (int i = 0; i < AttachedDeviceNumber; i++)
                {
                    String StrToAdd = GBMSAPI_Example_Util.GBMSAPI_Example_GetDevNameFromDevID(
                        AttachedDeviceList[i].DeviceID);
                    StrToAdd += " " + (AttachedDeviceList[i].DeviceSerialNumber);
                    this.DeviceTypeComboBox.Items.Add(StrToAdd);
                }
                if (AttachedDeviceNumber > 0)
                {
                    this.DeviceTypeComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in UpdateListButton_Click function: " + ex.Message);
                this.Close();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                this.Enabled = true;
            }
        }

        void GBMSAPIExample_InitMainForm()
        {
            try
            {
                //////////////////////////////
                // DISABLE POSSIBILITY TO
                // SAVE IMAGE (we don't have
                // any image to be saved)
                //////////////////////////////
                this.SaveFileStripButton.Enabled = false;

                //////////////////////////////
                // GET LIBRARY VERSION
                //////////////////////////////
                Byte VersionField1, VersionField2, VersionField3, VersionField4;
                GBMSAPI_NET_AuxiliaryRoutines.GBMSAPI_NET_GetMultiScanAPIVersion(
                    out VersionField1, out VersionField2, out VersionField3, out VersionField4
                    );

                this.GBMSAPIVersionTextBox.Text = "" + VersionField1 + "." + VersionField2 + "." + VersionField3 + "." + VersionField4;


                string LowLevelDllName;
                GBMSAPI_NET_AuxiliaryRoutines.GBMSAPI_NET_GetUnderlyingLibraryDllVersion(
                    out VersionField1, out VersionField2, out VersionField3, out VersionField4, out LowLevelDllName
                    );

                this.DeviceDllNameTextBox.Text = LowLevelDllName
                    + " Ver:" + VersionField1 + "." + VersionField2 + "." + VersionField3 + "." + VersionField4;

                //////////////////////////////
                // GET DEVICE STATISTICS
                //////////////////////////////
                uint Counter, ProductionDateInSec;
                GBMSAPI_NET_AuxiliaryRoutines.GBMSAPI_NET_GetScannerStatistics(out Counter, out ProductionDateInSec);

                TimeSpan TimeFromProductionDate = TimeSpan.FromSeconds((double)ProductionDateInSec);
                DateTime Reference = new DateTime(1970, 1, 1);
                DateTime ProductionDate = Reference.Add(TimeFromProductionDate);

                this.DeviceProductionDateTextBox.Text = ProductionDate.ToString();
                this.DeviceNumberOfUseTextBox.Text = "" + Counter;

                //////////////////////////////
                // GET DEVICE FW INFO
                //////////////////////////////
                String FwInfoString;
                int RetVal = GBMSAPI_NET_DeviceSettingRoutines.GBMSAPI_NET_GetDeviceNameAndVersion(
                    out FwInfoString);
                if (RetVal == GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_FEATURE_NOT_SUPPORTED)
                {
                    this.FwInfoTextBox.Text = "FEATURE NOT SUPPORTED";
                }
                else if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                {
                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal,
                        "InitMainForm,GBMSAPI_NET_GetDeviceNameAndVersion");
                    return;
                }
                else
                {
                    this.FwInfoTextBox.Text = FwInfoString;
                }

                //////////////////////////////
                // GET DEVICE FEATURES
                //////////////////////////////
                /*******************************
                * Device Features
                *******************************/
                uint DeviceFeaturesMask;
                RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetDeviceFeatures(out DeviceFeaturesMask);

                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                {
                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal, "InitMainForm,GBMSAPI_GetDeviceFeatures");
                    return;
                }

                // Multiple USB Connections
                if ((DeviceFeaturesMask & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_MULTIPLE_USB_CONNECTIONS_ALLOWED) != 0)
                {
                    this.MultipleUSBConnectionsLabel.BackColor = Color.Green;
                }
                else
                {
                    this.MultipleUSBConnectionsLabel.BackColor = Color.Red;
                }

                // 1000 DPI
                if ((DeviceFeaturesMask & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_1000DPI_RESOLUTION) != 0)
                {
                    this.ResolutionLabel.Text = "Image Resolution: 1000 DPI";
                }
                else
                {
                    this.ResolutionLabel.Text = "Image Resolution: 500 DPI";
                }

                // Auto capture blocking
                if ((DeviceFeaturesMask & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_AUTO_CAPTURE_BLOCKING) != 0)
                {
                    this.AutoCaptureBlockLabel.BackColor = Color.Green;
                }
                else
                {
                    this.AutoCaptureBlockLabel.BackColor = Color.Red;
                }

                // Image Rotation
                if ((DeviceFeaturesMask & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_IMAGE_ROTATION) != 0)
                {
                    this.ImageRotationLabel.BackColor = Color.Green;
                    this.ImageRotationGroupBox.Enabled = true;
                    this.RotationRadioButton_0.Checked = true;
                }
                else
                {
                    this.ImageRotationLabel.BackColor = Color.Red;
                    this.ImageRotationGroupBox.Enabled = false;
                }

                // VER 2700
                // Permanent Data Storage
                if ((DeviceFeaturesMask & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_PERMANENT_USER_DATA_STORAGE) != 0)
                {
                    this.PermanentDataStorageLabel.BackColor = Color.Green;
                    this.PermanentDataStorageToolStripButton.Enabled = true;
                }
                else
                {
                    this.PermanentDataStorageLabel.BackColor = Color.Red;
                    this.PermanentDataStorageToolStripButton.Enabled = false;
                }

                // VER 2700
                // Frame rate setting
                if ((DeviceFeaturesMask & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_FRAME_RATE_SETTING) != 0)
                {
                    this.FrameRateSettingDFLabel.BackColor = Color.Green;
                    this.FrameRateGroupBox.Enabled = true;
                }
                else
                {
                    this.FrameRateSettingDFLabel.BackColor = Color.Red;
                    this.FrameRateGroupBox.Enabled = false;
                    this.lFrameRatePreviewRange.Text = ("");
                    this.tbFrameRatePreview.Text = "";
                }

                // VER 2700
                // FW Retrieve info
                if ((DeviceFeaturesMask & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_FW_INFO_RETRIEVE) != 0)
                {
                    this.FwInfoRetrievingLabel.BackColor = Color.Green;
                }
                else
                {
                    this.FwInfoRetrievingLabel.BackColor = Color.Red;
                }

                // VER 2800
                if ((DeviceFeaturesMask & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_ROLL_AREA_IQS) != 0)
                {
                    this.RollIQSSupportDFLabel.BackColor = Color.Green;
                }
                else
                {
                    this.RollIQSSupportDFLabel.BackColor = Color.Red;
                }

                // VER 2800
                this.RollStandardCheckBox.Enabled = false;
                if ((DeviceFeaturesMask & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_ROLL_AREA_GA) != 0)
                {
                    this.RollGASupportDFLabel.BackColor = Color.Green;
                    this.RollStandardCheckBox.Enabled = true;
                }
                else
                {
                    this.RollGASupportDFLabel.BackColor = Color.Red;
                    this.RollStandardCheckBox.Enabled = false;
                }

                // VER 2.9.0.0
                if ((DeviceFeaturesMask & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_DRY_SKIN_IMG_ENHANCE) != 0)
                {
                    this.DrySkinEnhancementSupportDFLabel.BackColor = Color.Green;
                    this.EnhanceDrySkinImgCheckBox.Enabled = true;
                    // ver 3.4.0.0: enhance dry skin enabled by default
                    this.EnhanceDrySkinImgCheckBox.Checked = true;
                    this.EnhanceDrySkinImgCheckBox_CheckedChanged(null, null);
                    // end ver 3.4.0.0
                }
                else
                {
                    this.DrySkinEnhancementSupportDFLabel.BackColor = Color.Red;
                    this.EnhanceDrySkinImgCheckBox.Enabled = false;
                }

                // VER 3.1.0.0
                if ((DeviceFeaturesMask & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_ROLL_OBJECT_STRIPE) != 0)
                {
                    this.lSliceSettingSupport.BackColor = Color.Green;
                    this.gbStripeOptions.Enabled = true;
                    // VER 3.1.0.1
                    this.gbStripeOptions.Visible = true;
                    // end VER 3.1.0.1
                }
                else
                {
                    this.lSliceSettingSupport.BackColor = Color.Red;
                    this.gbStripeOptions.Enabled = false;
                    // VER 3.1.0.1
                    this.gbStripeOptions.Visible = false;
                    // end VER 3.1.0.1
                }
                // end VER 3.1.0.0

                // VER 3.2.0.0
                if ((DeviceFeaturesMask & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_ENABLE_BLOCK_ROLL_COMPOSITION) != 0)
                {
                    this.cbEnableRollCompositionBlock.Enabled = true;
                }
                else
                {
                    this.cbEnableRollCompositionBlock.Enabled = false;
                }
                // end VER 3.2.0.0

                // VER 3.4.0.0
                if ((DeviceFeaturesMask & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_USB_3_0_SUPPORT) != 0)
                {
                    this.lDeviceFeaturesUsb30Support.BackColor = Color.Green;
                }
                else
                {
                    this.lDeviceFeaturesUsb30Support.BackColor = Color.Red;
                }
                // end VER 3.4.0.0

                // VER 4.2.0.1
                this.tbUsbLinkType.Enabled = false;
                // end VER 4.2.0.1
                if ((DeviceFeaturesMask & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_USB_3_0_SUPPORT) != 0)
                {
                    this.lDeviceFeaturesUsb30Support.BackColor = Color.Green;
                    // VER 4.2.0.1
                    byte UsbLinkSpeed = 0;
                    RetVal = GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_GetUsbLinkSpeed(out UsbLinkSpeed);
                    if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                    {
                        GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal, "InitMainForm,GBMSAPI_GetUsbLinkSpeed");
                        this.tbUsbLinkType.Text = "ERROR";
                    }
                    else
                    {
                        this.tbUsbLinkType.Text = (UsbLinkSpeed == GBMSAPI_NET.GBMSAPI_NET_Defines.GBMSAPI_NET_DeviceCharacteristicsDefines.GBMSAPI_NET_UsbLinkValues.GBMSAPI_NET_USB_LINK_HIGH_SPEED) ? "2.0" : "3.0";
                    }
                    // end VER 4.2.0.1
                }
                else
                {
                    this.lDeviceFeaturesUsb30Support.BackColor = Color.Red;
                    // VER 4.2.0.1
                    this.tbUsbLinkType.Text = "NA";
                    // end VER 4.2.0.1
                }
                // end VER 3.4.0.0

                // VER 4.2.0.1
                if ((DeviceFeaturesMask & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_HEATER) != 0)
                {
                    this.lDeviceFeaturesHeater.BackColor = Color.Green;
                    this.gbHeaterSystem.Enabled = true;
                    RefreshHeaterControl();
                }
                else
                {
                    this.lDeviceFeaturesHeater.BackColor = Color.Red;
                    this.gbHeaterSystem.Enabled = false;
                }
                // end VER 4.2.0.1

                // VER 4.0.0.0
                this.rbHwFfdLowStrictness.Text = "Low (" + GBMSAPI_NET_HwAntifakeStrictnessThresholds.GBMSAPI_NET_HW_ANTIFAKE_THRESHOLD_LOW_STRICTNESS + ")";
                this.rbHwFfdMediumStrictness.Text = "Normal (" + GBMSAPI_NET_HwAntifakeStrictnessThresholds.GBMSAPI_NET_HW_ANTIFAKE_THRESHOLD_MEDIUM_STRICTNESS + ")";
                this.rbHwFfdHighStrictness.Text = "High (" + GBMSAPI_NET_HwAntifakeStrictnessThresholds.GBMSAPI_NET_HW_ANTIFAKE_THRESHOLD_HIGH_STRICTNESS + ")";
                this.rbHwFfdPersonalizedStrictness.Text = "Custom (0; " + GBMSAPI_NET_HwAntifakeStrictnessThresholds.GBMSAPI_NET_HW_ANTIFAKE_THRESHOLD_MAX + ")";
                this.cbEnableAutoCaptureBlockIfFakeFinger.Text = "Block Autocapture for Fake Fingers";
                this.tbHwFfdPersonalizedStrictValue.Text = "" + GBMSAPI_NET_HwAntifakeStrictnessThresholds.GBMSAPI_NET_HW_ANTIFAKE_THRESHOLD_MEDIUM_STRICTNESS;
                rbHwFfdPersonalizedStrictness_CheckedChanged(null, null);
                this.rbHwFfdMediumStrictness.Checked = true;
                if (((DeviceFeaturesMask & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_HW_ANTIFAKE) != 0) || ((DeviceFeaturesMask & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_SW_ANTIFAKE) != 0))
                {
                    this.gbFakeFingerDetectionSettings.Enabled = true;
                }
                else
                {
                    this.gbFakeFingerDetectionSettings.Enabled = false;
                }
                if ((DeviceFeaturesMask & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_HW_ANTIFAKE) != 0)
                {
                    this.lDeviceFeaturesHwFakeDetection.BackColor = Color.Green;
                    this.gbHwFfdSettings.Enabled = true;
                }
                else
                {
                    this.lDeviceFeaturesHwFakeDetection.BackColor = Color.Red;
                    this.gbHwFfdSettings.Enabled = false;
                }

                if ((DeviceFeaturesMask & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_SW_ANTIFAKE) != 0)
                {
                    this.lDeviceFeaturesSwFakeDetection.BackColor = Color.Green;
                    this.gbSwFfdSettings.Enabled = true;
                }
                else
                {
                    this.lDeviceFeaturesSwFakeDetection.BackColor = Color.Red;
                    this.gbSwFfdSettings.Enabled = false;
                }
                // end VER 4.0.0.0

                /*******************************
			    * Scan Options
			    *******************************/
                uint SupportedScanOptions;
                RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetSupportedScanOptions(out SupportedScanOptions);
                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                {
                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal, "InitMainForm,GBMSAPI_NET_GetSupportedScanOptions");
                    return;
                }

                // Auto capture
                if ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_AUTOCAPTURE) != 0)
                {
                    this.AutoCaptureLabel.BackColor = Color.Green;
                }
                else
                {
                    this.AutoCaptureLabel.BackColor = Color.Red;
                }

                // Full Resolution
                if ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_FULL_RES_PREVIEW) != 0)
                {
                    this.FullResolutionPreviewLabel.BackColor = Color.Green;
                }
                else
                {
                    this.FullResolutionPreviewLabel.BackColor = Color.Red;
                }

                // Flat single finger on roll scan area
                if ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_FLAT_SINGLE_FINGER_ON_ROLL_AREA) != 0)
                {
                    this.FlatSingleFingerOnRollAreaLabel.BackColor = Color.Green;
                }
                else
                {
                    this.FlatSingleFingerOnRollAreaLabel.BackColor = Color.Red;
                }

                // Manual roll preview stop
                if ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_MANUAL_ROLL_PREVIEW_STOP) != 0)
                {
                    this.PedalRollPreviewStopLabel.BackColor = Color.Green;
                }
                else
                {
                    this.PedalRollPreviewStopLabel.BackColor = Color.Red;
                }

                // No roll preview
                if ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_NO_ROLL_PREVIEW) != 0)
                {
                    this.NoRollPreviewLabel.BackColor = Color.Green;
                }
                else
                {
                    this.NoRollPreviewLabel.BackColor = Color.Red;
                }

                // High Speed Preview
                if ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_HIGH_SPEED_PREVIEW) != 0)
                {
                    this.HighSpeedPreviewLabel.BackColor = Color.Green;
                }
                else
                {
                    this.HighSpeedPreviewLabel.BackColor = Color.Red;
                }

                // VER 3.1.0.0
                if ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_ADAPT_ROLL_AREA_POSITION) != 0)
                {
                    this.lAutoRollPositioning.BackColor = Color.Green;
                }
                else
                {
                    this.lAutoRollPositioning.BackColor = Color.Red;
                }

                if ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_FORCE_ROLL_TO_LEFT) != 0)
                {
                    this.lAoForceRollToLeft.BackColor = Color.Green;
                }
                else
                {
                    this.lAoForceRollToLeft.BackColor = Color.Red;
                }

                if ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_FORCE_ROLL_TO_RIGHT) != 0)
                {
                    this.lAoForceRollToRight.BackColor = Color.Green;
                }
                else
                {
                    this.lAoForceRollToRight.BackColor = Color.Red;
                }

                if ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_EXTERNAL_ROLL_COMPOSITION) != 0)
                {
                    this.lAoExternalRollProcedure.BackColor = Color.Green;
                }
                else
                {
                    this.lAoExternalRollProcedure.BackColor = Color.Red;
                }
                // end VER 3.1.0.0



                /*******************************
                * Image Info
                *******************************/
                uint AvailableImageInfo;
                // Contrast and size: if available, they are valid for FLAT SINGLE FINGER
                RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetAvailableImageInfo(
                    GBMSAPI_NET_ScannableBiometricTypes.GBMSAPI_NET_SBT_FLAT_SINGLE_FINGER,
                    out AvailableImageInfo);
                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                {
                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal, "InitMainForm,GBMSAPI_NET_GetAvailableImageInfo");
                    return;
                }

                if ((AvailableImageInfo & GBMSAPI_NET_AvailableImageInfo.GBMSAPI_NET_AII_FINGERPRINT_CONTRAST) != 0)
                {
                    this.FPContrastAvailableLabel.BackColor = Color.Green;
                }
                else
                {
                    this.FPContrastAvailableLabel.BackColor = Color.Red;
                }

                if ((AvailableImageInfo & GBMSAPI_NET_AvailableImageInfo.GBMSAPI_NET_AII_FINGERPRINT_SIZE) != 0)
                {
                    this.FPSizeAvailableLabel.BackColor = Color.Green;
                }
                else
                {
                    this.FPSizeAvailableLabel.BackColor = Color.Red;
                }

                // HLP Completeness: if available, it's valid for LOWER HALF PALM
                uint ScannableTypesMask;
                RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetScannableTypes(out ScannableTypesMask);

                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                {
                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal, "InitMainForm,GBMSAPI_NET_GetScannableTypes");
                    return;
                }

                if ((ScannableTypesMask & GBMSAPI_NET_ScannableBiometricTypes.GBMSAPI_NET_SBT_FLAT_LOWER_HALF_PALM) != 0)
                {
                    RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetAvailableImageInfo(
                        GBMSAPI_NET_ScannableBiometricTypes.GBMSAPI_NET_SBT_FLAT_LOWER_HALF_PALM,
                        out AvailableImageInfo);
                    if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                    {
                        GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal, "InitMainForm,GBMSAPI_NET_GetAvailableImageInfo");
                        return;
                    }

                    if ((AvailableImageInfo & GBMSAPI_NET_AvailableImageInfo.GBMSAPI_NET_AII_LOWER_HALF_PALM_COMPLETENESS) != 0)
                    {
                        this.HLPCompletenessAvailable.BackColor = Color.Green;
                    }
                    else
                    {
                        this.HLPCompletenessAvailable.BackColor = Color.Red;
                    }
                }
                else
                {
                    this.HLPCompletenessAvailable.BackColor = Color.Red;
                }

                /*******************************
			    * External Equipment
			    *******************************/
                uint OptionalExternalEquipment;
                RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetOptionalExternalEquipment(out OptionalExternalEquipment);

                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                {
                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal, "InitMainForm,GBMSAPI_NET_GetOptionalExternalEquipment");
                    return;
                }

                // Indicator LEDs
                if ((OptionalExternalEquipment & GBMSAPI_NET_OptionalExternalEquipment.GBMSAPI_NET_OED_VUI_LED) != 0)
                {
                    this.VUILEDControlStripButton.Enabled = true;
                    this.IndicatorLEDAvailableLabel.BackColor = Color.Green;
                }
                else
                {
                    this.VUILEDControlStripButton.Enabled = false;
                    this.IndicatorLEDAvailableLabel.BackColor = Color.Red;
                }

                // Pedal
                if ((OptionalExternalEquipment & GBMSAPI_NET_OptionalExternalEquipment.GBMSAPI_NET_OED_PEDAL) != 0)
                {
                    this.PedalAvailableLabel.BackColor = Color.Green;
                }
                else
                {
                    this.PedalAvailableLabel.BackColor = Color.Red;
                }

                // Sound
                if ((OptionalExternalEquipment & GBMSAPI_NET_OptionalExternalEquipment.GBMSAPI_NET_OED_SOUND) != 0)
                {
                    this.SoundAvailableLabel.BackColor = Color.Green;
                }
                else
                {
                    this.SoundAvailableLabel.BackColor = Color.Red;
                }

                // LCD
                this.cbLcdLanguage.Items.Clear();
                if ((OptionalExternalEquipment & GBMSAPI_NET_OptionalExternalEquipment.GBMSAPI_NET_OED_VUI_LCD) != 0)
                {
                    this.LCDAvailableLabel.BackColor = Color.Green;
                    // Ver 2.8.0.0
                    uint LcdFeatures;
                    RetVal = GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_GetLcdFeatures(out LcdFeatures);
                    if ((LcdFeatures & GBMSAPI_NET_DisplayFeatures.GBMSAPI_NET_VILCD_LF_INTERMEDIATE_LIMIT) != 0)
                    {
                        this.lLcdIntermediateLimitSetting.BackColor = Color.Green;
                    }
                    else
                    {
                        this.lLcdIntermediateLimitSetting.BackColor = Color.Red;
                    }
                    if ((LcdFeatures & GBMSAPI_NET_DisplayFeatures.GBMSAPI_NET_VILCD_LF_LANGUAGE_SETTING) != 0)
                    {
                        this.lLcdLanguageSetting.BackColor = Color.Green;
                        this.cbLcdLanguage.Enabled = true;
                        this.cbLcdLanguage.Items.Add("en");
                        this.cbLcdLanguage.Items.Add("zh");
                        this.cbLcdLanguage.SelectedIndex = 0;
                    }
                    else
                    {
                        this.cbLcdLanguage.Enabled = false;
                        this.lLcdLanguageSetting.BackColor = Color.Red;
                    }
                }
                else
                {
                    this.LCDAvailableLabel.BackColor = Color.Red;
                    this.lLcdIntermediateLimitSetting.BackColor = Color.Red;
                    this.lLcdLanguageSetting.BackColor = Color.Red;
                }

                //////////////////////////////////
                // GET SCANNABLE TYPES
                //////////////////////////////////
                this.ScannableObjectTypesListBox.Items.Clear();

                List<String> ScannableTypesList = GBMSAPI_Example_Util.GetScannableTypesListFromMask(ScannableTypesMask);
                if (ScannableTypesList != null && ScannableTypesList.Count > 0)
                {
                    for (int i = 0; i < ScannableTypesList.Count; i++)
                    {
                        this.ScannableObjectTypesListBox.Items.Add(ScannableTypesList[i]);
                    }
                }

                /////////////////////////////////////
                // GET MAX IMAGE SIZE
                /////////////////////////////////////
                uint ImgMaxSizeX, ImgMaxSizeY;
                RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetMaxImageSize(out ImgMaxSizeX, out ImgMaxSizeY);

                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                {
                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal, "InitMainForm,GBMSAPI_NET_GetMaxImageSize");
                    return;
                }

                // allocate buffers for acquisition
                GBMSAPI_Example_Globals.AcquisitionPreviewBuffer = Marshal.AllocHGlobal((int)(ImgMaxSizeX * ImgMaxSizeY));
                GBMSAPI_Example_Globals.AcquisitionFullResBuffer = Marshal.AllocHGlobal((int)(ImgMaxSizeX * ImgMaxSizeY));
                if (GBMSAPI_Example_Globals.AcquisitionPreviewBuffer == IntPtr.Zero ||
                    GBMSAPI_Example_Globals.AcquisitionFullResBuffer == IntPtr.Zero)
                {
                    MessageBox.Show("GBMSAPIExample_InitMainForm: Memory allocation failed", "FATAL ERROR");
                    return;
                }

                /////////////////////////////////////
                // GET CALIBRATION IMAGE SIZE
                /////////////////////////////////////
                // ver 2.10.0.0: allocate buffer into calibration form directly
                //uint CalImgSizeX, CalImgSizeY;
                //RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetCalibrationImageSize(
                //    true, out CalImgSizeX, out CalImgSizeY
                //);

                //if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                //{
                //    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal, "InitMainForm,GBMSAPI_NET_GetCalibrationImageSize");
                //    return;
                //}

                //GBMSAPI_Example_Globals.CalibrationBuffer = Marshal.AllocHGlobal((int)(CalImgSizeX * CalImgSizeY));
                //GBMSAPI_Example_Globals.DummyCalibrationBuffer = Marshal.AllocHGlobal((int)(CalImgSizeX * CalImgSizeY));
                //if (GBMSAPI_Example_Globals.CalibrationBuffer == IntPtr.Zero ||
                //    GBMSAPI_Example_Globals.DummyCalibrationBuffer == IntPtr.Zero)
                //{
                //    MessageBox.Show("GBMSAPIExample_InitMainForm: Memory allocation failed", "FATAL ERROR");
                //    return;
                //}
                if (GBMSAPI_Example_Globals.CalibrationBuffer != IntPtr.Zero) Marshal.FreeHGlobal(GBMSAPI_Example_Globals.CalibrationBuffer);
                if (GBMSAPI_Example_Globals.DummyCalibrationBuffer != IntPtr.Zero)
                    Marshal.FreeHGlobal(GBMSAPI_Example_Globals.DummyCalibrationBuffer);
                // ver 2.10.0.0: allocate buffer into calibration form directly

                // VER 2.10.0.0 Scan Areas
                //////////////////////////////
                // GET Scan Areas
                //////////////////////////////
                UInt32 scanArea;
                RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetSupportedScanAreas(out scanArea);
                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                {
                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal, "InitMainForm,GBMSAPI_NET_GetSupportedScanAreas");
                    return;
                }

                // Full Frame
                if ((scanArea & GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_FULL_FRAME) != 0)
                {
                    this.lScanAreaFullFrame.BackColor = Color.Green;
                }
                else
                {
                    this.lScanAreaFullFrame.BackColor = Color.Red;
                }

                // roll iqs
                if ((scanArea & GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_ROLL_IQS) != 0)
                {
                    this.lScanAreaRollIqs.BackColor = Color.Green;
                    // VER 3.1.0.0
                    this.cbStripeIqsArea.Checked = false;
                    // end VER 3.1.0.0
                }
                else
                {
                    this.lScanAreaRollIqs.BackColor = Color.Red;
                }

                // ROLL GA
                if ((scanArea & GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_ROLL_GA) != 0)
                {
                    this.lScanAreaRollGa.BackColor = Color.Green;
                }
                else
                {
                    this.lScanAreaRollGa.BackColor = Color.Red;
                }

                // PHOTO
                if ((scanArea & GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_PHOTO) != 0)
                {
                    this.lScanAreaPhoto.BackColor = Color.Green;
                }
                else
                {
                    this.lScanAreaPhoto.BackColor = Color.Red;
                }

                // ROLL JOINT
                if ((scanArea & GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_ROLL_JOINT) != 0)
                {
                    this.lScanAreaRollJoint.BackColor = Color.Green;
                    // VER 3.1.0.0
                    this.cbStripeJointArea.Checked = false;
                    // end VER 3.1.0.0
                }
                else
                {
                    this.lScanAreaRollJoint.BackColor = Color.Red;
                }

                // ROLL THENAR
                if ((scanArea & GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_ROLL_THENAR) != 0)
                {
                    this.lScanAreaRollThenar.BackColor = Color.Green;
                    // VER 3.1.0.0
                    this.cbStripeThenarArea.Checked = false;
                    // end VER 3.1.0.0
                }
                else
                {
                    this.lScanAreaRollThenar.BackColor = Color.Red;
                }
                // end VER 2.10.0.0 Scan Areas

                ///////////////////////////////////////
                // LOAD OBJECTS TO SCAN LIST
                ///////////////////////////////////////
                this.ObjectToScanComboBox.Items.Clear();

                // check all bits of ScannableTypesMask
                for (int i = 0; i < 32; i++)
                {
                    uint Mask = ((uint)1) << i;

                    Mask &= ScannableTypesMask;

                    if (Mask != 0)
                    {
                        // bit is set
                        String[] ObjList = GBMSAPI_Example_Util.GetObjectsToScanListFromObjectType(Mask);
                        foreach (String ObjToScan in ObjList)
                        {
                            this.ObjectToScanComboBox.Items.Add(ObjToScan);
                        }
                    }
                }
                if (this.ObjectToScanComboBox.Items.Count > 0)
                {
                    this.ObjectToScanComboBox.SelectedIndex = 0;
                }
                this.ObjectToScanComboBox_SelectedIndexChanged(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in UpdateListButton_Click function: " + ex.Message);
                this.Close();
            }
            finally
            {
            }
        }

        private void ObjectToScanComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.RefreshControls();
        }

        // ver 3.1.0.0
        // ver 3.1.0.0

        private void RefreshControls()
        {
            try
            {
                uint ObjToScanType, scanArea, OptionMask, FrameRateOptions;
                if (GetAcquisitionSettingsOptions(out ObjToScanType, out OptionMask, out scanArea, out FrameRateOptions))
                {
                    ///////////////////////////////
                    // LOAD IMAGE SIZE
                    ///////////////////////////////
                    this.ObjToScanImageSizeListBox.Items.Clear();
                    uint PreviewImgMaxSizeX, PreviewImgMaxSizeY;
                    uint FullResImgMaxSizeX, FullResImgMaxSizeY;

                    /*******************************
                     * Get ImageSize
                     * ****************************/
                    // ver 3.1.0.0: use GetImageSize3
                    int RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetImageSize3(
                        ObjToScanType, OptionMask, scanArea,
                        out FullResImgMaxSizeX, out FullResImgMaxSizeY,
                        out PreviewImgMaxSizeX, out PreviewImgMaxSizeY);
                    if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                    {
                        GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                            RetVal, "ObjectToScanComboBox_SelectedIndexChanged,GBMSAPI_NET_GetImageSize3");
                        return;
                    }
                    this.ObjToScanImageSizeListBox.Items.Add(
                        ("PREVIEW SIZE X = " + PreviewImgMaxSizeX + "; PREVIEW SIZE Y = " + PreviewImgMaxSizeY)
                        );
                    this.ObjToScanImageSizeListBox.Items.Add(
                        ("FULL SIZE X = " + FullResImgMaxSizeX + "; FULL SIZE Y = " + FullResImgMaxSizeY)
                        );

                    ////////////////////////////////////
                    // LOAD SCAN OPTIONS
                    ////////////////////////////////////
                    uint SupportedScanOptions;
                    RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetSupportedScanOptions(out SupportedScanOptions);
                    if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                    {
                        GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                            RetVal, "ObjectToScanComboBox_SelectedIndexChanged,GBMSAPI_NET_GetSupportedScanOptions");
                        return;
                    }

                    // ver 3.1.0.0: enable FullRresolutionInPreviewCheckBox only if object is not rolled
                    if (!GBMSAPI_Example_Util.IsRolled(ObjToScanType))
                    {
                        this.FullRresolutionInPreviewCheckBox.Enabled =
                            ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_FULL_RES_PREVIEW) != 0) ?
                            true : false;
                    }
                    else
                    {
                        this.FullRresolutionInPreviewCheckBox.Enabled = false;
                    }
                    // end ver 3.1.0.0

                    this.FullRresolutionInPreviewCheckBox.Enabled =
                        ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_FULL_RES_PREVIEW) != 0) ?
                        true : false;

                    this.HighSpeedCheckBox.Enabled =
                        ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_HIGH_SPEED_PREVIEW) != 0) ?
                        true : false;

                    // check if Roll or Flat GroupBox should be enabled
                    // VER 3.1.0.0: check all rolled objects
                    if (GBMSAPI_Example_Util.IsRolled(ObjToScanType))
                    // end VER 3.1.0.0: check all rolled objects
                    {
                        this.FlatObjectOptionsGroupBox.Enabled = false;
                        this.RollObjectOptionsGroupBox.Enabled = true;// no roll preview

                        this.NoRollPreviewCheckBox.Enabled =
                            ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_NO_ROLL_PREVIEW) != 0) ?
                        true : false;

                        // Ver 3.1.0.0: Roll Autoposition
                        this.cbRollAutoPositioningInPreview.Enabled =
                            ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_ADAPT_ROLL_AREA_POSITION) != 0);
                        this.cbExternalRollProcedure.Enabled =
                            ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_EXTERNAL_ROLL_COMPOSITION) != 0);
                        this.gbRollDirection.Enabled =
                            ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_FORCE_ROLL_TO_LEFT) != 0);
                        // end Ver 3.1.0.0: Roll Autoposition

                        // manual roll preview stop mode
                        this.PedalRollPreviewStopModeRadioButton.Enabled =
                            ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_MANUAL_ROLL_PREVIEW_STOP) != 0) ?
                            true : false;
                        this.PedalRollPreviewStopModeRadioButton.Checked = false;

                        this.SideRollPreviewStopModeRadioButton.Enabled = true;
                        this.CenterRollPreviewStopModeRadioButton.Enabled = true;

                        // by default, side roll preview is enabled
                        this.SideRollPreviewStopModeRadioButton.Checked = true;
                    }
                    else
                    {
                        this.FlatObjectOptionsGroupBox.Enabled = true;
                        this.RollObjectOptionsGroupBox.Enabled = false;

                        // Auto-Capture
                        this.AutoCaptureCheckBox.Enabled =
                            ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_AUTOCAPTURE) != 0) ?
                            true : false;
                        if (this.AutoCaptureCheckBox.Enabled) this.AutoCaptureCheckBox.Checked = true;

                        // Flat single finger on Roll area
                        if (ObjToScanType == GBMSAPI_NET_ScannableBiometricTypes.GBMSAPI_NET_SBT_FLAT_SINGLE_FINGER &&
                            ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_FLAT_SINGLE_FINGER_ON_ROLL_AREA) != 0)
                            )
                        {
                            this.FlatSingleFingerOnRollAreaCheckBox.Enabled = true;
                        }
                        else
                        {
                            this.FlatSingleFingerOnRollAreaCheckBox.Enabled = false;
                        }
                    }

                    if (this.FrameRateGroupBox.Enabled)
                    {
                        LoadFrameRateRange();
                        // ver 4.0.0.0
                        this.SetFrameRateFromControls(false);
                        // end ver 4.0.0.0
                    }

                    /////////////////////////
                    // LOAD DISPLAY OPTIONS
                    /////////////////////////
                    uint OptionalExternalEquipment;
                    RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetOptionalExternalEquipment(out OptionalExternalEquipment);

                    if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                    {
                        GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                            RetVal, "ObjectToScanComboBox_SelectedIndexChanged,GBMSAPI_NET_GetOptionalExternalEquipment");
                        return;
                    }

                    if ((OptionalExternalEquipment & GBMSAPI_NET_OptionalExternalEquipment.GBMSAPI_NET_OED_VUI_LCD) != 0)
                    {
                        // Ver 2.8.0.0
                        // Intermediate limit enabling
                        uint LcdFeatures;
                        RetVal = GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_GetLcdFeatures(out LcdFeatures);
                        // Screen after acquisition
                        this.StopScreenAfterAcquisitionCheckBox.Enabled = true;

                        if (
                            // VER 3.1.0.0: check all rolled objects
                            !(GBMSAPI_Example_Util.IsRolled(ObjToScanType))
                            // end VER 3.1.0.0: check all rolled objects
                        )
                        {
                            this.ShowCustomizedContrastCheckBox.Enabled = true;
                            this.ContrastLimitTextBox.Enabled = true;
                            this.ContrastLimitTextBox.Text = "120";
                            if ((LcdFeatures & GBMSAPI_NET_DisplayFeatures.GBMSAPI_NET_VILCD_LF_INTERMEDIATE_LIMIT) != 0)
                            {
                                this.tbContrastIntermediateLimit.Enabled = true;
                                this.tbContrastIntermediateLimit.Text = "90";
                            }
                            else
                            {
                                this.tbContrastIntermediateLimit.Enabled = false;
                            }
                        }
                        else
                        {
                            this.ShowCustomizedContrastCheckBox.Enabled = false;
                            this.ContrastLimitTextBox.Enabled = false;
                            this.tbContrastIntermediateLimit.Enabled = false;
                        }

                        // completeness
                        if (ObjToScanType == GBMSAPI_NET_ScannableBiometricTypes.GBMSAPI_NET_SBT_FLAT_LOWER_HALF_PALM)
                        {
                            this.ShowCustomizedCompletenessCheckBox.Enabled = true;
                            this.CompletenessLimitTextBox.Enabled = true;
                            this.CompletenessLimitTextBox.Text = "90";
                            if ((LcdFeatures & GBMSAPI_NET_DisplayFeatures.GBMSAPI_NET_VILCD_LF_INTERMEDIATE_LIMIT) != 0)
                            {
                                this.tbCompleteIntermediateLimit.Enabled = true;
                                this.tbCompleteIntermediateLimit.Text = "60";
                            }
                            else
                            {
                                this.tbCompleteIntermediateLimit.Enabled = false;
                            }
                        }
                        else
                        {
                            this.ShowCustomizedCompletenessCheckBox.Enabled = false;
                            this.CompletenessLimitTextBox.Enabled = false;
                            this.tbCompleteIntermediateLimit.Enabled = false;
                        }
                    }
                    else
                    {
                        this.ShowCustomizedCompletenessCheckBox.Enabled = false;
                        this.ShowCustomizedContrastCheckBox.Enabled = false;
                        this.ContrastLimitTextBox.Enabled = false;
                        this.CompletenessLimitTextBox.Enabled = false;
                        this.StopScreenAfterAcquisitionCheckBox.Enabled = false;
                        this.tbCompleteIntermediateLimit.Enabled = false;
                        this.tbContrastIntermediateLimit.Enabled = false;
                    }

                    ////////////////////////////
                    // LOAD CLIP OPTIONS
                    ////////////////////////////
                    // Clipping
                    if (ObjToScanType != GBMSAPI_NET_ScannableBiometricTypes.GBMSAPI_NET_SBT_PHOTO)
                    {
                        this.ClipEnableCheckBox.Enabled = true;
                    }
                    else
                    {
                        this.ClipEnableCheckBox.Enabled = false;
                    }
                    this.ClipEnableCheckBox_CheckedChanged(null, null);
                    // ver 4.0.0.0
                    this.ClipRegionSizeXTextBox_Leave(null, null);
                    this.ClipRegionSizeYTextBox_Leave(null, null);
                    // end ver 4.0.0.0

                    ////////////////////////////
                    // LOAD DrySkin Enhance
                    // option
                    ////////////////////////////
                    this.EnhanceDrySkinImgCheckBox_CheckedChanged(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in UpdateListButton_Click function: " + ex.Message);
                this.Close();
            }
        }

        private void CalibrateDeviceToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                CalibrationForm CalWindow = new CalibrationForm();
                if (CalWindow != null)
                {
                    CalWindow.ShowDialog();
                    CalWindow.ReleaseResources();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception in CalibrateDeviceToolStripButton_Click: " + ex.Message);
            }
        }

        private void VUILEDControlStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                VUI_LED DlgToOpen = new VUI_LED();
                if (DlgToOpen != null)
                {
                    DlgToOpen.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception in VUILEDControlStripButton_Click: " + ex.Message);
            }
        }

        private void AcquireImageButton_Click(object sender, EventArgs e)
        {
            AcquisitionForm DlgToOpen = new AcquisitionForm(this);

            try
            {
                GBMSAPI_Example_Globals.DSInit(GBMSAPI_Example_Globals.COINIT_APARTMENTTHREADED, false);
                try
                {
                    if (DlgToOpen != null)
                    {
                        DlgToOpen.ShowDialog();
                    }
                    Bitmap LastAcqImage = null;

                    // copy image data
                    IntPtr SourcePtr = ((GBMSAPI_Example_Globals.LastEventInfo &
                        GBMSAPI_NET_EventInfo.GBMSAPI_NET_EI_ACQUISITION_PHASE) == 0) ?
                        GBMSAPI_Example_Globals.AcquisitionPreviewBuffer :
                        GBMSAPI_Example_Globals.AcquisitionFullResBuffer;

                    int SourceSX, SourceSY;

                    if (
                     GBMSAPI_Example_Globals.ClippingRegionPosX >= 0 &&
                     GBMSAPI_Example_Globals.ClippingRegionPosY >= 0 &&
                     GBMSAPI_Example_Globals.ClippingRegionSizeX != 0 &&
                     GBMSAPI_Example_Globals.ClippingRegionSizeY != 0
                     )
                    {
                        // clip the resulting image
                        Byte[] TempBuffer = new Byte[GBMSAPI_Example_Globals.LastFrameSizeX * GBMSAPI_Example_Globals.LastFrameSizeY];
                        Marshal.Copy(SourcePtr, TempBuffer, 0, TempBuffer.Length);
                        SourceSX = (int)GBMSAPI_Example_Globals.ClippingRegionSizeX;
                        SourceSY = (int)GBMSAPI_Example_Globals.ClippingRegionSizeY;

                        Byte[] Clipped = GBMSAPI_Example_Util.CutImage(
                            TempBuffer,
                            GBMSAPI_Example_Globals.LastFrameSizeX, GBMSAPI_Example_Globals.LastFrameSizeY,
                            GBMSAPI_Example_Globals.ClippingRegionPosX, GBMSAPI_Example_Globals.ClippingRegionPosY,
                            SourceSX, SourceSY);

                        Marshal.Copy(Clipped, 0, SourcePtr, Clipped.Length);
                    }
                    else
                    {
                        SourceSX = GBMSAPI_Example_Globals.LastFrameSizeX;
                        SourceSY = GBMSAPI_Example_Globals.LastFrameSizeY;
                    }

                    LastAcqImage = new Bitmap(
                             SourceSX, SourceSY,
                             SourceSX,
                             System.Drawing.Imaging.PixelFormat.Format8bppIndexed,
                             SourcePtr);

                    ColorPalette pal = LastAcqImage.Palette;
                    for (int i = 0; i < pal.Entries.Length; i++)
                    {
                        pal.Entries[i] = Color.FromArgb(255, i, i, i);
                    }
                    LastAcqImage.Palette = pal;

                    // Rescale Acquired image picture box, holding its width,
                    // and varying the height (until a 300 pixel threshold, then fix width)
                    this.AcquiredImagePictureBox.Width = GBMSAPI_Example_Globals.DesiredPictureBoxSize;
                    this.AcquiredImagePictureBox.Height = (int)(
                        ((double)(this.AcquiredImagePictureBox.Width)) *
                        ((double)(LastAcqImage.Height) / (double)(LastAcqImage.Width))
                        );
                    if (this.AcquiredImagePictureBox.Height > GBMSAPI_Example_Globals.DesiredPictureBoxSize)
                    {
                        this.AcquiredImagePictureBox.Height = GBMSAPI_Example_Globals.DesiredPictureBoxSize;
                        this.AcquiredImagePictureBox.Width = (int)(
                            ((double)(this.AcquiredImagePictureBox.Height)) *
                            ((double)(LastAcqImage.Width) / (double)(LastAcqImage.Height))
                            );
                    }

                    this.AcquiredImagePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    this.AcquiredImagePictureBox.Image = LastAcqImage;

                    this.ImSXTextBox.Text = "" + LastAcqImage.Width;
                    this.ImSYTextBox.Text = "" + LastAcqImage.Height;
                    this.ContrastTextBox.Text = "" + GBMSAPI_Example_Globals.ImageContrast;
                    this.SizeTextBox.Text = "" + GBMSAPI_Example_Globals.ImageSize;
                    this.ArtefactsSizeTextBox.Text = "" + GBMSAPI_Example_Globals.RolledArtefactSize;

                    // clear resources
                    DlgToOpen.ReleaseResources();

                    // enable possibility to save
                    this.SaveFileStripButton.Enabled = true;

                    this.Update();
                }
                catch (System.ObjectDisposedException)
                {
                    // next instruction just to avoid compiler warnings
                    MessageBox.Show("No scanners found or some error occurred in AcquisitionForm");
                    DlgToOpen.ReleaseResources();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception in AcquireImageButton_Click: " + ex.Message);
                DlgToOpen.ReleaseResources();
            }
        }

        private void SelectDeviceButton_Click(object sender, EventArgs e)
        {
            String CurrentChoosenDev = (String)(this.DeviceTypeComboBox.SelectedItem);

            if (CurrentChoosenDev != null)
            {
                // Parse string: <DevName><Space character><DevSerialNumber>
                int SpacePosition = CurrentChoosenDev.IndexOf(" ");
                String DevName = CurrentChoosenDev.Substring(0, SpacePosition);

                String DevSerialNumber = CurrentChoosenDev.Substring(SpacePosition + 1);

                if (DevName != null && DevSerialNumber != null)
                {
                    Byte DevID = GBMSAPI_Example_Util.GBMSAPI_Example_GetDevIDFromDevName(DevName);

                    if (DevID != 0)
                    {
                        this.Enabled = false;
                        Cursor.Current = Cursors.WaitCursor;
                        Application.DoEvents();
                        int RetVal =
                            GBMSAPI_NET.GBMSAPI_NET_LibraryFunctions.GBMSAPI_NET_DeviceSettingRoutines.GBMSAPI_NET_SetCurrentDevice
                            (DevID, DevSerialNumber);
                        Cursor.Current = Cursors.Default;
                        this.Enabled = true;
                        if (RetVal == GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                        {
                            this.GBMSAPIExample_InitMainForm();
                        }
                        else
                        {
                            GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal, "SelectDeviceForm OK Button");
                        }
                    }
                }
            }
        }

        private void SaveFileStripButton_Click(object sender, EventArgs e)
        {
            // copy image data
            IntPtr SourcePtr = ((GBMSAPI_Example_Globals.LastEventInfo &
                GBMSAPI_NET_EventInfo.GBMSAPI_NET_EI_ACQUISITION_PHASE) == 0) ?
               GBMSAPI_Example_Globals.AcquisitionPreviewBuffer :
               GBMSAPI_Example_Globals.AcquisitionFullResBuffer;

            // create bitmap
            Bitmap LastAcqImage = new Bitmap(GBMSAPI_Example_Globals.LastFrameSizeX,
                GBMSAPI_Example_Globals.LastFrameSizeY,
                GBMSAPI_Example_Globals.LastFrameSizeX,
                System.Drawing.Imaging.PixelFormat.Format8bppIndexed,
                (IntPtr)SourcePtr);

            System.Drawing.Imaging.ColorPalette pal = LastAcqImage.Palette;
            for (int i = 0; i < pal.Entries.Length; i++)
            {
                pal.Entries[i] = Color.FromArgb(255, i, i, i);
            }
            LastAcqImage.Palette = pal;
            // check image resolution
            uint DevFeatures;
            int RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetDeviceFeatures(out DevFeatures);
            if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
            {
                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                    RetVal, "SaveFileStripButton_Click,GBMSAPI_GetDeviceFeatures");
                return;
            }
            // VER 4.2.0.0: set resolution
            if ((DevFeatures & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_1000DPI_RESOLUTION) != 0)
            {
                LastAcqImage.SetResolution((float)(1000), (float)(1000));
            }
            else
            {
                LastAcqImage.SetResolution((float)(500), (float)(500));
            }
            // end VER 4.2.0.0: set resolution
            LastAcqImage.Save("FullResImage.bmp", System.Drawing.Imaging.ImageFormat.Bmp);



            if ((DevFeatures & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_1000DPI_RESOLUTION) != 0)
            {
                System.Windows.Forms.DialogResult res =
                MessageBox.Show("Saved Image is in 1000 dpi resolution: would you like to save it at a 500 dpi also?",
                    "1000 DPI option",
                    MessageBoxButtons.YesNo);

                if (res == System.Windows.Forms.DialogResult.Yes)
                {
                    //////////////////
                    // save in 500 dpi
                    //////////////////
                    Byte[] SourceArray = new Byte[GBMSAPI_Example_Globals.LastFrameSizeX * GBMSAPI_Example_Globals.LastFrameSizeY];
                    if (SourceArray == null)
                    {
                        MessageBox.Show("Error in memory allocation: close application", "FATAL ERROR");
                        return;
                    }
                    Marshal.Copy(SourcePtr, SourceArray, 0, SourceArray.Length);

                    Byte[] DestinationArray;

                    // convert to 500 DPI
                    RetVal = GBMSAPI_NET_ImageProcessingUtilities.GBMSAPI_NET_ConvertImageFrom1000to500DPI(
                        SourceArray,
                        out DestinationArray,
                        (uint)GBMSAPI_Example_Globals.LastFrameSizeX,
                        (uint)GBMSAPI_Example_Globals.LastFrameSizeY);

                    if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                    {
                        GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                            RetVal, "SaveFileStripButton_Click,GBMSAPI_ConvertImageFrom1000to500DPI");
                        return;
                    }

                    IntPtr DestinationPointer = Marshal.AllocHGlobal(DestinationArray.Length);
                    if (DestinationPointer == IntPtr.Zero)
                    {
                        MessageBox.Show("Error in memory allocation: close application", "FATAL ERROR");
                        return;
                    }
                    Marshal.Copy(DestinationArray, 0, DestinationPointer, DestinationArray.Length);

                    // create bitmap
                    Bitmap DecimatedAcqImage = new Bitmap(GBMSAPI_Example_Globals.LastFrameSizeX / 2,
                        GBMSAPI_Example_Globals.LastFrameSizeY / 2,
                        GBMSAPI_Example_Globals.LastFrameSizeX / 2,
                        System.Drawing.Imaging.PixelFormat.Format8bppIndexed,
                        (IntPtr)DestinationPointer);

                    System.Drawing.Imaging.ColorPalette DestinationPal = DecimatedAcqImage.Palette;
                    for (int i = 0; i < DestinationPal.Entries.Length; i++)
                    {
                        DestinationPal.Entries[i] = Color.FromArgb(255, i, i, i);
                    }
                    DecimatedAcqImage.Palette = DestinationPal;

                    DecimatedAcqImage.Save("500DPIImage.bmp");
                }
            }
        }

        private void ClipEnableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ClipEnableCheckBox.Checked == true)
            {
                this.ClipRegionSizeXTextBox.Enabled = true;
                this.ClipRegionSizeYTextBox.Enabled = true;
            }
            else
            {
                this.ClipRegionSizeXTextBox.Enabled = false;
                this.ClipRegionSizeYTextBox.Enabled = false;
            }
        }

        private void ClipRegionSizeXTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                int ClSX = Int32.Parse(this.ClipRegionSizeXTextBox.Text);
                if (ClSX < 200)
                {
                    MessageBox.Show("Min Clip Size X is 200: changing it to the min value", "WARNING");
                    this.ClipRegionSizeXTextBox.Text = "200";
                }
                // ver 4.0.0.0
                uint ObjToScanType, scanArea, OptionMask, FrameRateOptions;
                GetAcquisitionSettingsOptions(out ObjToScanType, out OptionMask, out scanArea, out FrameRateOptions);

                uint PreviewImgMaxSizeX, PreviewImgMaxSizeY;
                uint FullResImgMaxSizeX, FullResImgMaxSizeY;

                int RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetImageSize3(
                    ObjToScanType, OptionMask, scanArea,
                    out FullResImgMaxSizeX, out FullResImgMaxSizeY,
                    out PreviewImgMaxSizeX, out PreviewImgMaxSizeY);
                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                {
                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                        RetVal, "ClipRegionSizeXTextBox_Leave,GBMSAPI_NET_GetImageSize3");
                    return;
                }
                if (ClSX >= FullResImgMaxSizeX)
                {
                    MessageBox.Show("Clip Size X exceeds Image Size X: changing it to the Image Size X value", "WARNING");
                    this.ClipRegionSizeXTextBox.Text = "" + (FullResImgMaxSizeX - 1);
                }
                // end ver 4.0.0.0
            }
            catch (Exception)
            {
            }
        }

        private void ClipRegionSizeYTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                int ClSY = Int32.Parse(this.ClipRegionSizeYTextBox.Text);
                if (ClSY < 200)
                {
                    MessageBox.Show("Min Clip Size Y is 200: changing it to the min value", "WARNING");
                    this.ClipRegionSizeYTextBox.Text = "200";
                }
                // ver 4.0.0.0
                uint ObjToScanType, scanArea, OptionMask, FrameRateOptions;
                GetAcquisitionSettingsOptions(out ObjToScanType, out OptionMask, out scanArea, out FrameRateOptions);

                uint PreviewImgMaxSizeX, PreviewImgMaxSizeY;
                uint FullResImgMaxSizeX, FullResImgMaxSizeY;

                int RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetImageSize3(
                    ObjToScanType, OptionMask, scanArea,
                    out FullResImgMaxSizeX, out FullResImgMaxSizeY,
                    out PreviewImgMaxSizeX, out PreviewImgMaxSizeY);
                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                {
                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                        RetVal, "ClipRegionSizeYTextBox_Leave,GBMSAPI_NET_GetImageSize3");
                    return;
                }
                if (ClSY >= FullResImgMaxSizeY)
                {
                    MessageBox.Show("Clip Size Y exceeds Image Size Y: changing it to the Image Size Y value", "WARNING");
                    this.ClipRegionSizeYTextBox.Text = "" + (FullResImgMaxSizeY - 1);
                }
                // end ver 4.0.0.0
            }
            catch (Exception)
            {
            }
        }

        private void NoRollPreviewCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.NoRollPreviewCheckBox.Checked == false)
            {
                this.RollPreviewOptionsGroupBox.Enabled = true;
                // ver 3.1.0.0
                uint SupportedScanOptions;
                GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetSupportedScanOptions(out SupportedScanOptions);
                this.cbRollAutoPositioningInPreview.Enabled =
                    ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_ADAPT_ROLL_AREA_POSITION) != 0);
                this.cbExternalRollProcedure.Enabled =
                    ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_EXTERNAL_ROLL_COMPOSITION) != 0);
                // end ver 3.1.0.0
            }
            else
            {
                this.RollPreviewOptionsGroupBox.Enabled = false;
                // ver 3.1.0.0
                this.cbExternalRollProcedure.Enabled = false;
                this.cbExternalRollProcedure.Checked = false;
                this.cbRollAutoPositioningInPreview.Enabled = false;
                this.cbRollAutoPositioningInPreview.Checked = false;
                // end ver 3.1.0.0
            }
        }

        private void NoRollPreviewCheckBox_EnabledChanged(object sender, EventArgs e)
        {
            if (this.NoRollPreviewCheckBox.Enabled == false)
            {
                this.RollPreviewOptionsGroupBox.Enabled = true;
                // ver 3.1.0.0
                uint SupportedScanOptions;
                GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetSupportedScanOptions(out SupportedScanOptions);
                this.cbRollAutoPositioningInPreview.Enabled =
                    ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_ADAPT_ROLL_AREA_POSITION) != 0);
                this.cbExternalRollProcedure.Enabled =
                    ((SupportedScanOptions & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_EXTERNAL_ROLL_COMPOSITION) != 0);
                // end ver 3.1.0.0
            }
        }

        private void DeviceTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SelectDeviceButton_Click(sender, e);
        }

        private void FlatSingleFingerOnRollAreaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.RefreshControls();
        }

        private void RotationRadioButton_0_CheckedChanged(object sender, EventArgs e)
        {
            if (this.RotationRadioButton_0.Checked == true)
            {
                int RetVal = GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_SetImageRotation(
                    GBMSAPI_NET_ImageRotationValues.GBMSAPI_NET_IMG_ROTATE_NO);
                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                {
                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                        RetVal, "RotationRadioButton_0_CheckedChanged");
                }
            }
        }

        private void RotationRadioButton_90_CheckedChanged(object sender, EventArgs e)
        {
            if (this.RotationRadioButton_90.Checked == true)
            {
                int RetVal = GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_SetImageRotation(
                    GBMSAPI_NET_ImageRotationValues.GBMSAPI_NET_IMG_ROTATE_90_CW);
                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                {
                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                        RetVal, "RotationRadioButton_90_CheckedChanged");
                }
            }
        }

        private void RotationRadioButton_180_CheckedChanged(object sender, EventArgs e)
        {
            if (this.RotationRadioButton_180.Checked == true)
            {
                int RetVal = GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_SetImageRotation(
                    GBMSAPI_NET_ImageRotationValues.GBMSAPI_NET_IMG_ROTATE_180);
                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                {
                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                        RetVal, "RotationRadioButton_180_CheckedChanged");
                }
            }
        }

        private void RotationRadioButton_270_CheckedChanged(object sender, EventArgs e)
        {
            if (this.RotationRadioButton_270.Checked == true)
            {
                int RetVal = GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_SetImageRotation(
                    GBMSAPI_NET_ImageRotationValues.GBMSAPI_NET_IMG_ROTATE_90_CCW);
                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                {
                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                        RetVal, "RotationRadioButton_270_CheckedChanged");
                }
            }
        }

        private void SelectImageTimeoutTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                UInt32 TimeoutToSelect = UInt32.Parse(this.SelectImageTimeoutTextBox.Text);

                int RetVal = GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_SetSelectImageTimeout(TimeoutToSelect);
                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                {
                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                        RetVal, "SelectImageTimeoutTextBox_Leave");
                }
            }
            catch (Exception)
            {
            }
        }

        private void PermanentDataStorageToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                PermanentDataStorageForm PermWindow = new PermanentDataStorageForm();
                if (PermWindow != null)
                {
                    PermWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception in PermanentDataStorageToolStripButton_Click: " + ex.Message);
            }
        }

        private void FullRresolutionInPreviewCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.RefreshControls();
        }

        private void LoadFrameRateRange()
        {
            double prevMaxFR, prevMinFR, prevDefFR, fullresMaxFR, fullresMinFR, fullresDefFR;

            prevMaxFR = prevMinFR = prevDefFR = fullresMaxFR = fullresMinFR = fullresDefFR = 0;
            ///////////////////////////
            // Load Options
            ///////////////////////////
            uint ObjToScanType, scanArea, OptionMask, FrameRateOptions;
            if (GetAcquisitionSettingsOptions(out ObjToScanType, out OptionMask, out scanArea, out FrameRateOptions))
            {
                ///////////////////////////
                // Get Frame Rate Range
                ///////////////////////////
                Byte devId;
                string devSerNum;
                int RetVal = GBMSAPI_NET_DeviceSettingRoutines.GBMSAPI_NET_GetCurrentDevice(out devId, out devSerNum);
                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                {
                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                        RetVal, "LoadFrameRateRange,GBMSAPI_NET_GetCurrentDevice");
                    this.lFrameRatePreviewRange.Text = ("");
                    this.tbFrameRatePreview.Text = "";
                    return;
                }
                FrameRateOptions &= ~(GBMSAPI_NET_FrameRateOptions.GBMSAPI_NET_FRO_FULL_RESOLUTION_MODE);
                RetVal = GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_GetFrameRateRange2(
                    devId, FrameRateOptions, scanArea,
                    out prevMaxFR, out prevMinFR, out prevDefFR
                    );
                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                {
                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                        RetVal, "LoadFrameRateRange,GBMSAPI_NET_GetFrameRateRange3");
                    this.lFrameRatePreviewRange.Text = ("");
                    this.tbFrameRatePreview.Text = "";
                    return;
                }
                FrameRateOptions |= (GBMSAPI_NET_FrameRateOptions.GBMSAPI_NET_FRO_FULL_RESOLUTION_MODE);
                RetVal = GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_GetFrameRateRange2(
                    devId, FrameRateOptions, scanArea,
                    out fullresMaxFR, out fullresMinFR, out fullresDefFR
                    );
                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                {
                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                        RetVal, "LoadFrameRateRange,GBMSAPI_NET_GetFrameRateRange3");
                    this.lFrameRatePreviewRange.Text = ("");
                    this.tbFrameRatePreview.Text = "";
                    return;
                }

                this.lFrameRatePreviewRange.Text = ("" + prevMinFR +
                        " < FR < " + prevMaxFR);
                this.tbFrameRatePreview.Text = "" + prevDefFR;
                this.lFrameRateFullResRange.Text = ("" + fullresMinFR +
                        " < FR < " + fullresMaxFR);
                this.tbFrameRateFullRes.Text = "" + fullresDefFR;
            }
        }

        private void SetFrameRateFromControls(bool ShowConfirmation)
        {
            if (this.tbFrameRatePreview.Enabled)
            {

                double prevMaxFR, prevMinFR, prevDefFR, fullresMaxFR, fullresMinFR, fullresDefFR;

                prevMaxFR = prevMinFR = prevDefFR = fullresMaxFR = fullresMinFR = fullresDefFR = 0;
                ///////////////////////////
                // Load Options
                ///////////////////////////
                uint ObjToScanType, scanArea, OptionMask, FrameRateOptions;
                if (GetAcquisitionSettingsOptions(out ObjToScanType, out OptionMask, out scanArea, out FrameRateOptions))
                {
                    ///////////////////////////
                    // Get Frame Rate Range
                    ///////////////////////////
                    Byte devId;
                    string devSerNum;
                    int RetVal = GBMSAPI_NET_DeviceSettingRoutines.GBMSAPI_NET_GetCurrentDevice(out devId, out devSerNum);
                    if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                    {
                        GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                            RetVal, "SetFrameRateButton_Click,GBMSAPI_NET_GetCurrentDevice");
                        this.lFrameRatePreviewRange.Text = ("");
                        this.tbFrameRatePreview.Text = "";
                        return;
                    }
                    FrameRateOptions &= ~(GBMSAPI_NET_FrameRateOptions.GBMSAPI_NET_FRO_FULL_RESOLUTION_MODE);
                    RetVal = GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_GetFrameRateRange2(
                        devId, FrameRateOptions, scanArea,
                        out prevMaxFR, out prevMinFR, out prevDefFR
                        );
                    if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                    {
                        GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                            RetVal, "SetFrameRateButton_Click,GBMSAPI_NET_GetFrameRateRange3");
                        this.lFrameRatePreviewRange.Text = ("");
                        this.tbFrameRatePreview.Text = "";
                        return;
                    }
                    FrameRateOptions |= (GBMSAPI_NET_FrameRateOptions.GBMSAPI_NET_FRO_FULL_RESOLUTION_MODE);
                    RetVal = GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_GetFrameRateRange2(
                        devId, FrameRateOptions, scanArea,
                        out fullresMaxFR, out fullresMinFR, out fullresDefFR
                        );
                    if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                    {
                        GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                            RetVal, "SetFrameRateButton_Click,GBMSAPI_NET_GetFrameRateRange3");
                        this.lFrameRatePreviewRange.Text = ("");
                        this.tbFrameRatePreview.Text = "";
                        return;
                    }

                    Double previewFrameRate, fullresFrameRate;
                    if (!double.TryParse(this.tbFrameRatePreview.Text, out previewFrameRate))
                    {
                        MessageBox.Show("Preview Frame Rate Value must be a number");
                        this.tbFrameRatePreview.Text = "";
                        return;
                    }
                    if (previewFrameRate > prevMaxFR || previewFrameRate < prevMinFR)
                    {
                        MessageBox.Show("Preview Frame Rate Value out of range");
                        this.tbFrameRatePreview.Text = "";
                        return;
                    }
                    if (!double.TryParse(this.tbFrameRateFullRes.Text, out fullresFrameRate))
                    {
                        MessageBox.Show("Full Resolution Frame Rate Value must be a number");
                        this.tbFrameRateFullRes.Text = "";
                        return;
                    }
                    if (fullresFrameRate > fullresMaxFR || fullresFrameRate < fullresMinFR)
                    {
                        MessageBox.Show("Full Resolution Frame Rate Value out of range");
                        this.tbFrameRateFullRes.Text = "";
                        return;
                    }

                    FrameRateOptions &= ~(GBMSAPI_NET_FrameRateOptions.GBMSAPI_NET_FRO_FULL_RESOLUTION_MODE);
                    RetVal = GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_SetFrameRate2(
                        scanArea, FrameRateOptions, previewFrameRate);
                    if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                    {
                        GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                            RetVal, "SetFrameRateButton_Click,GBMSAPI_NET_SetFrameRate3");
                        this.tbFrameRatePreview.Text = "";
                        this.tbFrameRateFullRes.Text = "";
                        return;
                    }
                    FrameRateOptions |= (GBMSAPI_NET_FrameRateOptions.GBMSAPI_NET_FRO_FULL_RESOLUTION_MODE);
                    RetVal = GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_SetFrameRate2(
                        scanArea, FrameRateOptions, fullresFrameRate);
                    if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                    {
                        GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                            RetVal, "SetFrameRateButton_Click,GBMSAPI_NET_SetFrameRate3");
                        this.tbFrameRatePreview.Text = "";
                        this.tbFrameRateFullRes.Text = "";
                        return;
                    }
                    if (ShowConfirmation) MessageBox.Show("Frame Rate correctly set");
                }
                else
                {
                    MessageBox.Show("Cannot load options for frame rate setting");
                }
            }
        }

        private void SetFrameRateButton_Click(object sender, EventArgs e)
        {
            SetFrameRateFromControls(true);
        }

        private void RollStandardCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            RefreshControls();
        }

        private void RollStandardCheckBox_EnabledChanged(object sender, EventArgs e)
        {
            if (this.RollStandardCheckBox.Enabled)
            {
                this.RollStandardCheckBox.Checked = true;
            }
        }

        private void cbLcdLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                String LanguageToSet = (String)this.cbLcdLanguage.SelectedItem;
                if (LanguageToSet != null)
                {
                    GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_SetLanguage(LanguageToSet);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception in cbLcdLanguage_SelectedIndexChanged: " + ex.Message);
            }
        }

        private void GBMASPI_CsExampleMainForm_Shown(object sender, EventArgs e)
        {
            UpdateListButton_Click(null, null);
            this.GBMSAPIExample_InitMainForm();
        }

        // VER 2.9.0.0
        private void EnhanceDrySkinImgCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_EnableDrySkinImgEnhance(EnhanceDrySkinImgCheckBox.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception in EnhanceDrySkinImgCheckBox_CheckedChanged: " + ex.Message);
            }
        }

        // VER 3.1.0.0
        private void bEnableRollStripeAcquisition_Click(object sender, EventArgs e)
        {
            int RetVal;
            ///////////////////////////////////
            // IQS AREA
            ///////////////////////////////////
            if (this.cbStripeIqsArea.Checked)
                RetVal = GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_ROLL_EnableRollStripeAcquisition(
                    GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_ROLL_IQS, true);
            else
                RetVal = GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_ROLL_EnableRollStripeAcquisition(
                    GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_ROLL_IQS, false);
            if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
            {
                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                    RetVal, "bEnableRollStripeAcquisition_Click,GBMSAPI_NET_ROLL_EnableRollStripeAcquisition");
                return;
            }
            ///////////////////////////////////
            // JOINT AREA
            ///////////////////////////////////
            if (this.cbStripeJointArea.Checked)
                RetVal = GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_ROLL_EnableRollStripeAcquisition(
                    GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_ROLL_JOINT, true);
            else
                RetVal = GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_ROLL_EnableRollStripeAcquisition(
                    GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_ROLL_JOINT, false);
            if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
            {
                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                    RetVal, "bEnableRollStripeAcquisition_Click,GBMSAPI_NET_ROLL_EnableRollStripeAcquisition");
                return;
            }
            ///////////////////////////////////
            // THENAR AREA
            ///////////////////////////////////
            if (this.cbStripeThenarArea.Checked)
                RetVal = GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_ROLL_EnableRollStripeAcquisition(
                    GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_ROLL_THENAR, true);
            else
                RetVal = GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_ROLL_EnableRollStripeAcquisition(
                    GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_ROLL_THENAR, false);
            if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
            {
                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                    RetVal, "bEnableRollStripeAcquisition_Click,GBMSAPI_NET_ROLL_EnableRollStripeAcquisition");
                return;
            }
        }

        private void cbRollAutoPositioningInPreview_CheckedChanged(object sender, EventArgs e)
        {
            if (cbRollAutoPositioningInPreview.Enabled) this.RefreshControls();
        }
        // end VER 3.1.0.0

        // ver 4.0.0.0
        private void rbHwFfdPersonalizedStrictness_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbHwFfdPersonalizedStrictness.Checked)
            {
                this.tbHwFfdPersonalizedStrictValue.Enabled = true;
            }
            else
            {
                this.tbHwFfdPersonalizedStrictValue.Enabled = false;
            }
        }
        // end ver 4.0.0.0

        // ver 4.1.0.0
        private void cbEnableAutoCaptureBlockIfFakeFinger_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEnableAutoCaptureBlockIfFakeFinger.Checked)
            {
                GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_EnableAutoCaptureBlockForDetectedFakes((byte)1);
            }
            else GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_EnableAutoCaptureBlockForDetectedFakes((byte)0);
        }
        // end ver 4.1.0.0

        // ver 4.2.0.1
        private void RefreshHeaterControl()
        {
            this.tbHeaterInfo.Text = "ERROR";
            this.lHeaterRange.Text = "ERROR";
            this.tBarHeaterTemp.Value = 0;
            this.tBarHeaterTemp.SetRange(0, 100);
            this.tBarHeaterTemp.Enabled = false;
            this.bHeaterSetTemp.Enabled = false;
            this.bHeaterSetTemp.Text = "NA";
            int Result;
            int RetVal = GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_CheckHeater(out Result);
            if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
            {
                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal, "RefreshHeaterControl,GBMSAPI_CheckHeater");
            }
            else
            {
                if (Result != GBMSAPI_NET_HeaterCheckResults.GBMSAPI_NET_HEATER_CHECK_RESULT_OK)
                {
                    if (Result == GBMSAPI_NET_HeaterCheckResults.GBMSAPI_NET_HEATER_CHECK_RESULT_FAIL)
                    {
                        MessageBox.Show("RefreshHeaterControl, GBMSAPI_CheckHeater Result = FAIL");
                    }
                    else
                    {
                        MessageBox.Show("RefreshHeaterControl, GBMSAPI_CheckHeater Result = " + Result);
                    }
                }
                else
                {
                    float TempC;
                    RetVal = GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_GetHeaterMeanTemp(out TempC);
                    if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                    {
                        GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal, "RefreshHeaterControl,GBMSAPI_GetHeaterMeanTemp");
                    }
                    else
                    {
                        this.tbHeaterInfo.Text = "TEMP=" + TempC;
                        float tMax, tMin, tDef;
                        GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_GetheaterMeanTempRange(out tMax, out tDef, out tMin);
                        this.lHeaterRange.Text = "Range: [" + tMin + "," + tMax + "]";
                        this.tBarHeaterTemp.Enabled = true;
                        this.tBarHeaterTemp.SetRange(0, 100);
                        float Val = ((float)100.0 / (tMax - tMin)) * (TempC - tMin);
                        this.tBarHeaterTemp.Value = (int)Val;
                        this.bHeaterSetTemp.Enabled = true;
                        this.bHeaterSetTemp.Text = "Set to " + TempC;
                    }
                }
            }
        }
        private void tBarHeaterTemp_Scroll(object sender, EventArgs e)
        {
            float TempC = (float)(this.tBarHeaterTemp.Value);
            float tMax, tMin, tDef;
            GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_GetheaterMeanTempRange(out tMax, out tDef, out tMin);

            TempC = tMin + (TempC * (tMax - tMin) / 100);
            this.bHeaterSetTemp.Text = "Set to " + TempC;
        }

        private void bHeaterSetTemp_Click(object sender, EventArgs e)
        {
            int RetVal;
            float TempC = (float)(this.tBarHeaterTemp.Value);
            float tMax, tMin, tDef;
            GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_GetheaterMeanTempRange(out tMax, out tDef, out tMin);

            TempC = tMin + (TempC * (tMax - tMin) / 100);
            //MessageBox.Show("TempC = " + TempC);
            RetVal = GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_SetHeaterMeanTemp(TempC);
            if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
            {
                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal, "bHeaterSetTemp_Click,GBMSAPI_SetHeaterMeanTemp");
            }
            else
            {
                RefreshHeaterControl();
            }
        }
        // end ver 4.2.0.1
    }
}