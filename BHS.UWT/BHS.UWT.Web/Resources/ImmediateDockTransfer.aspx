<%--Modifications
Task	| By	| Date          | Modification Description
--------|-------|---------------|-------------------------
36833   | TDA   | 09/29/08      | Created
58804	| JAG	| 09/22/09		| Added support to transfer a multi order pallet
--%>

<%@ Page Language="C#" AutoEventWireup="true" %>

<%@ OutputCache Duration="900" VaryByParam="*" VaryByHeader="Accept-Language" %>
<%@ Implements Interface="Manh.ILS.Shipping.RF.Interface.IImmediateDockTransfer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">

<%--import assemblies--%>
<%@ Import Namespace="Manh.WMFW.General" %>
<%@ Import Namespace="Manh.WMW.General" %>
<%@ Import Namespace="Manh.ILS.General" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Manh.ILS.Shipping.Interfaces" %>

<script type="text/C#" runat="server">

	//Variables Controls
    string _carrier = string.Empty;
    string _containerID = string.Empty;
    string _custName = string.Empty;
    string _destLoc = string.Empty;
    string _shipId = string.Empty;
    string _shipLoadNum = string.Empty;
    string _shipTo = string.Empty;
	
	string _btnCancel = string.Empty;
	string _btnTransfer = string.Empty;
	string _title = string.Empty;

	//Error Messages
    string _contErrorMsg = string.Empty;
    string _destLocErrorMsg = string.Empty;
    string _msgContainerNotExist = string.Empty;

	string _multipleEntriesText = string.Empty;
	
    string _colon = string.Empty;
	string _msgSuccess = string.Empty;
	
	//URI Information for Get and Dock Transfer
    string _uriGetShippingContainer = string.Empty;
	string _uriGetMultiOrderPallet = string.Empty;
	double _processStartDateTime;

	public void Dispose()
	{
		if(ThreadManager.CurrentSession != null)
			ThreadManager.CurrentSession.Dispose();
	}
	
	public void Page_Load(object sender, EventArgs e)
	{
		try
		{
			this.BuildSession();
			this.TranslateText();
			this.BuildUri();
		}
		catch (Exception eX)
		{
		    //Log the exception
		    IExceptionManager exceptionManager = SpringNetFactory.GetObject("IExceptionManager") as IExceptionManager;
		    exceptionManager.LogException(eX);

		    //Redirect to the Error Page
		    this.Context.Response.Redirect("TechnicalError.aspx?ReturnURL=ImmediateDockTransfer.aspx&session=" + this.Request["session"]);
		}

	}
    
	public void CheckUserAuthorizationOnActions() 
	{
		
	}
    
	public void BuildUri()
	{

		IImmediateDockTransferPresenter immediateDockTransferPresenter = SpringNetFactory.GetObject("IImmediateDockTransferPresenter") 
			as IImmediateDockTransferPresenter;

		this._uriGetShippingContainer = immediateDockTransferPresenter.GetContainerNoOpenWorkUri();
		this._uriGetMultiOrderPallet = immediateDockTransferPresenter.GetMultiOrderPalletUri();
	}

	public void BuildSession()
	{
		ISessionFactory sessionFactory = SpringNetFactory.GetObject("ISessionFactory") as ISessionFactory;
		ThreadManager.CurrentSession = sessionFactory.CreateSessionFromWebRequestString(this.Request["Session"]);
		DateTime jsStartDate = new DateTime(1970, 1, 1);
		TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - jsStartDate.Ticks);
		_processStartDateTime = Convert.ToInt64(ts.TotalMilliseconds);
	}

	public void TranslateText()
	{
		IResourceManager resourceManager = SpringNetFactory.GetObject("IResourceManager") as IResourceManager;
		this._containerID = resourceManager.GetStringResource("RF_CONTAINERID", ResourceGroups.Text);
        this._destLoc = resourceManager.GetStringResource("RF_DESTINATIONLOCATION", ResourceGroups.Text);
        this._shipId = resourceManager.GetStringResource("RF_SHIPMENTID", ResourceGroups.Text);
        this._shipLoadNum = resourceManager.GetStringResource("RF_SHIPLOADNUM", ResourceGroups.Text);
        this._carrier = resourceManager.GetStringResource("RF_CARRIER", ResourceGroups.Text);
        this._custName = resourceManager.GetStringResource("RF_CUSTNAME", ResourceGroups.Text);
        this._shipTo = resourceManager.GetStringResource("RF_SHIPTO", ResourceGroups.Text);
        
		this._btnCancel = resourceManager.GetStringResource("BTN_CANCEL", ResourceGroups.Text);
		this._btnTransfer = resourceManager.GetStringResource("BTN_TRANSFER", ResourceGroups.Text);

        this._title = resourceManager.GetStringResource("MNU_RFIMMEDIATEDOCKTRANSFER", ResourceGroups.Text);
        
		this._contErrorMsg = resourceManager.GetStringResource("MSG_CONT04", ResourceGroups.Msg);
        this._destLocErrorMsg = resourceManager.GetStringResource("MSG_DESTINATIONLOC01", ResourceGroups.Msg);
		this._colon = resourceManager.GetStringResource("COLON", ResourceGroups.Text);
        this._msgSuccess = resourceManager.GetStringResource("MSG_DOCKTRANSFER01", ResourceGroups.Msg);
		this._msgContainerNotExist = resourceManager.GetStringResource("MSG_CONT02", ResourceGroups.Msg);

		this._multipleEntriesText = resourceManager.GetStringResource("RFIMMDTRANSMULTIPLE", ResourceGroups.Text);
		
	}
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
      
	<script type="text/javascript" src="/RF/js/using.js" ></script>
	<script type="text/javascript" src="/RF/js/registerCommonDependencies.js" ></script>
	<script type="text/javascript" src="/RF/js/httpClient.js" ></script>
	<script type="text/javascript" src="/RF/js/immediateDockTransfer.js" ></script>
	
	<!-- added bhs -->
    	<script type="text/javascript" src="js/json2.js"></script>
    	
    	<style>
    	
    	    input[type=text],input[type=password]
    	    {
    	        width: 125px;
    	    }
    	    
    	    #mainTable
    	    {
    	        width: 210px;
    	    }
    	    
    	    #mainTable td
    	    {
    	        padding: 0px;
    	    }
    	    
    	
    	</style>
    	
	<title><%=_title%></title>
</head>
<body>
	<div id="wrapper">
		<div id="header"><%=_title%></div>
		<hr class="formDivider" />
		<div id="content">
			<table id="mainTable">
				<tr>
					<td class="labelColumn">
						<label><%=_containerID%><%=_colon%></label>
					</td>
					<td class="textInputColumn" >
						<input id="txtBoxContainerId" class="uppercase" type="text" />
					</td>
				</tr>
				<tr>
					<td class="labelColumn">
						<label><%=_destLoc%><%=_colon%></label>
					</td>
					<td class="textInputColumn">
						<input id="txtBoxDestLoc" class="uppercase" type="text" />
					</td>
				</tr>
				<tr>
					<td class="labelColumn">
						<label>Check Digit:</label>
					</td>
					<td class="textInputColumn">
					    <input id="txtBoxCheckDigit" type="password" />
					</td>
				</tr>
			</table>
			<hr class="formDivider" />
            <div id="displayOnlyContent">
              <table>
                <tr>
                  <td class="labelColumn">
                    <label><%=_shipId%><%=_colon%></label>
                  </td>
                  <td class="displayOnlyTextContentColumn">
                    <label id="shipIdValue"></label>
                  </td>
                </tr>
                <tr>
                  <td class="labelColumn">
                    <label><%=_shipLoadNum%><%=_colon%></label>
                  </td>
                  <td class="displayOnlyTextContentColumn">
                    <label id="shipLoadNumValue"></label>
                  </td>
                </tr>
                <tr>
                  <td class="labelColumn">
                    <label><%=_custName%><%=_colon%></label>
                  </td>
                  <td class="displayOnlyTextContentColumn">
                    <label id="custNameValue"></label>
                  </td>
                </tr>
                <tr>
                  <td class="labelColumn">
                    <label><%=_shipTo%><%=_colon%></label>
                  </td>
                  <td class="displayOnlyTextContentColumn">
                    <label id="shipToValue"></label>
                  </td>
                </tr>
                <tr>
                  <td class="labelColumn">
                    <label><%=_carrier%><%=_colon%></label>
                  </td>
                  <td class="displayOnlyTextContentColumn">
                    <label id="carrierValue"></label>
                  </td>
                </tr>
                <tr>
                  <td class="labelColumn">
                    <label>Container<%=_colon%></label>
                  </td>
                  <td class="displayOnlyTextContentColumn">
                    <label id="containersValue"></label>
                  </td>
                </tr>
             </table>
            </div> 
		</div>
		<hr class="formDivider" />
		<div class="buttons">
			<input id="buttonSubmit" type="submit" value="<%=_btnTransfer %>" class="button" />
			<input id="buttonCancel" type="button" value="<%=_btnCancel %>" class="button" />
		</div>
	</div>
	<div id="hiddenFields">
		<input id="ContErrorMsg" type="hidden" value="<%=_contErrorMsg%>" />
		<input id="DestLocErrorMsg" type="hidden" value="<%=_destLocErrorMsg%>" />
		<input id="ShippingContainerUri" type="hidden" value="<%=_uriGetShippingContainer%>" />
		<input id="MultiOrderPalletUri" type="hidden" value="<%=_uriGetMultiOrderPallet%>" />
		<input id="SuccessMsg" type="hidden" value="<%=_msgSuccess%>" />
		<input id="ContainerNotExist" type="hidden" value="<%=_msgContainerNotExist%>" />
		<input id="MultipleEntriesText" type="hidden" value="<%=_multipleEntriesText%>" />
		<input id="ProcessStartTime" type="hidden" value="<%=_processStartDateTime%>" />
		
		
		<input id="BHSUserName" type="hidden" value="<%= Session["user"].ToString() %>" />
		<input id="BHSEnvironment" type="hidden" value="<%=Session["environment"].ToString() %>" />
	</div>
</body>
</html>
