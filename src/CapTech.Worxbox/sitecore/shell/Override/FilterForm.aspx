<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FieldEditor.aspx.cs" Inherits="Sitecore.Shell.Applications.ContentManager.FieldEditorPage" %>

<%@ Import Namespace="Sitecore.Globalization" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.HtmlControls" Assembly="Sitecore.Kernel" %>
<asp:placeholder id="DocumentType" runat="server" />

<html>
<head id="Head1" runat="server">
    <meta http-equiv="X-Frame-Options" content="SAMEORIGIN">
    <asp:PlaceHolder ID="BrowserTitle" runat="server" />
    <sc:Stylesheet ID="Stylesheet1" runat="server" Src="Content Manager.css" DeviceDependant="true" />
    <asp:PlaceHolder ID="Stylesheets" runat="server" />

    <script type="text/JavaScript" language="javascript" src="/sitecore/shell/controls/SitecoreObjects.js"></script>
    <script type="text/JavaScript" language="javascript" src="/sitecore/shell/controls/SitecoreKeyboard.js"></script>
    <script type="text/JavaScript" language="javascript" src="/sitecore/shell/controls/SitecoreVSplitter.js"></script>

    <script type="text/JavaScript" language="javascript" src="/sitecore/shell/Applications/Content Manager/Content Editor.js"></script>
    <script type="text/JavaScript" language="javascript" src="/sitecore/shell/controls/TreeviewEx/TreeviewEx.js"></script>

    <script type="text/javascript">
        function OnResize() {
            var header = $("HeaderRow");
            var footer = $("FooterRow");

            var editor = $("MainPanel");

            var height = window.innerHeight - header.getHeight() - footer.getHeight() + 'px';

            editor.setStyle({ height: height });
        }

        // todo: required by chrome. See if there's a way to make this work without JS
        Event.observe(window, "load", OnResize);
        Event.observe(window, "resize", OnResize);

        if (scForm) {
            scForm.enableModifiedHandling();
        }
    </script>

    <style type="text/css">
        html, body {
            overflow: hidden;
        }



        #EditorPanel, .scEditorPanelCell {
            padding-bottom: 1px;
        }

        .scEditorPanelCell {
            padding-bottom: 1px;
        }

        .ie #ValidatorPanel {
            margin-top: 2px;
        }

        .scEditorSections {
            margin-right: 11px;
            background-color: transparent;
        }

        .ff .scEditorSections {
            margin-top: -2px;
        }

        #HeaderRow {
            display: none;
        }

        #FooterRow {
            position: relative;
            background-color: #f0f0f0;
            border-top: 1px solid #e3e3e3;
            padding: 8px 15px 9px;
            height: 56px;
            -moz-box-sizing: border-box;
            box-sizing: border-box;
            line-height: 34px;
        }

            #FooterRow > div {
                white-space: nowrap;
                position: absolute;
                right: 15px;
                text-align: right;
            }

        .scEditorSectionPanelCell {
            padding-left: 8px;
        }


        .scEditorSectionCaptionExpanded {
            padding: 1px 2px 1px 2px;
        }

        #WarningRow {
            background: #ffffe4;
            padding: 2px;
            font-weight: bolder;
        }

        .scEditorPanel {
            overflow-y: auto;
        }

        .novalidators .scEditorPanel {
          margin-right: 0;
        }

        tr {
            vertical-align: top;
        }

    </style>
</head>
<body runat="server" id="Body">
    <form id="ContentEditorForm" style="" runat="server">
        <sc:CodeBeside ID="CodeBeside1" runat="server" Type="Sitecore.sitecore.shell.Applications.Workbox.CommentEditor, Sitecore.Client" />
        <asp:PlaceHolder ID="scLanguage" runat="server" />
        <asp:Literal runat="server" EnableViewState="false" ID="CustomParamsContainer"></asp:Literal>
        <input type="hidden" id="scEditorTabs" name="scEditorTabs" />
        <input type="hidden" id="scActiveEditorTab" name="scActiveEditorTab" />
        <input type="hidden" id="scPostAction" name="scPostAction" />
        <input type="hidden" id="scShowEditor" name="scShowEditor" />
        <input type="hidden" id="scSections" name="scSections" />
        <div id="outline" class="scOutline" style="display: none"></div>
        <span id="scPostActionText" style="display: none">
            <sc:Literal ID="Literal1" Text="The main window could not be updated due to the current browser security settings. You must click the Refresh button yourself to view the changes." runat="server" />
        </span>
        <iframe id="feRTEContainer" src="about:blank" style="position: absolute; width: 100%; height: 100%; top: 0px; left: 0px; right: 0px; bottom: 0px; z-index: 999; border: none; display: none" frameborder="0" allowtransparency="allowtransparency"></iframe>
        <div class="scStretch scFlexColumnContainer">
            <div id="HeaderRow">
                <table cellpadding="0" cellspacing="0" style="background: white">
                    <tr>
                        <td>
                            <sc:ThemedImage Margin="4px 8px 4px 8px" ID="DialogIcon" Src="people/32x32/cubes_blue.png" runat="server" Height="32" Width="32" />
                        </td>

                        <td valign="top" width="100%">
                            <div style="padding: 2px 16px 0px 0px;">
                                <div style="padding: 0px 0px 4px 0px; font: bold 9pt tahoma; color: black">
                                    <sc:Literal Text="Field Editor" ID="DialogTitle" runat="server" />
                                </div>
                                <div style="color: #333333">
                                    <sc:Literal ID="DialogText" Text="Edit the fields" runat="server" />
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
            <div visible="False" id="WarningRow" runat="server">
                <sc:ThemedImage ID="ThemedImage1" runat="server" Height="16" Width="16" Style="vertical-align: middle; margin-right: 4px" Src="Applications/16x16/warning.png" />
                <sc:Literal ID="warningText" runat="server"></sc:Literal>
            </div>
            <div id="MainPanel" class="scFlexContent" onclick="javascript:scContent.onEditorClick(this, event);">
                <div id="MainContent" class="scStretchAbsolute">
                    <sc:Border ID="ContentEditor" runat="server" Class="scEditor" Style="margin-top: -1px" />
                </div>
            </div>
            <div id="FooterRow">
                <div>
                    <asp:Literal runat="server" ID="Buttons" />
                </div>
            </div>
        </div>
        <sc:KeyMap ID="KeyMap1" runat="server" />
    </form>
</body>
</html>
