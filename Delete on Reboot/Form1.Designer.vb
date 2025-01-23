<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtFile = New System.Windows.Forms.TextBox()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.chkAddToContextMenu = New System.Windows.Forms.CheckBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btnStageRename = New System.Windows.Forms.Button()
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnRemoveItem = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnBrowse = New System.Windows.Forms.Button()
        Me.listOperations = New System.Windows.Forms.ListView()
        Me.colFileName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colRenamedTo = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(82, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Location of File:"
        '
        'txtFile
        '
        Me.txtFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFile.Location = New System.Drawing.Point(100, 6)
        Me.txtFile.Name = "txtFile"
        Me.txtFile.Size = New System.Drawing.Size(643, 20)
        Me.txtFile.TabIndex = 1
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'chkAddToContextMenu
        '
        Me.chkAddToContextMenu.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkAddToContextMenu.AutoSize = True
        Me.chkAddToContextMenu.Location = New System.Drawing.Point(538, 325)
        Me.chkAddToContextMenu.Name = "chkAddToContextMenu"
        Me.chkAddToContextMenu.Size = New System.Drawing.Size(242, 17)
        Me.chkAddToContextMenu.TabIndex = 4
        Me.chkAddToContextMenu.Text = "Add ""Delete on Reboot"" to File Context Menu"
        Me.chkAddToContextMenu.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(9, 74)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(132, 13)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Current Staged Operations"
        '
        'btnStageRename
        '
        Me.btnStageRename.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnStageRename.Image = Global.Delete_on_Reboot.My.Resources.Resources.rename
        Me.btnStageRename.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnStageRename.Location = New System.Drawing.Point(475, 32)
        Me.btnStageRename.Name = "btnStageRename"
        Me.btnStageRename.Size = New System.Drawing.Size(154, 23)
        Me.btnStageRename.TabIndex = 9
        Me.btnStageRename.Text = "Stage Rename on Reboot"
        Me.btnStageRename.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnStageRename.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnSave.Enabled = False
        Me.btnSave.Image = Global.Delete_on_Reboot.My.Resources.Resources.save
        Me.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSave.Location = New System.Drawing.Point(113, 321)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(151, 23)
        Me.btnSave.TabIndex = 8
        Me.btnSave.Text = "Save Pending Operations"
        Me.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnRemoveItem
        '
        Me.btnRemoveItem.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnRemoveItem.Image = Global.Delete_on_Reboot.My.Resources.Resources.removeSmall
        Me.btnRemoveItem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnRemoveItem.Location = New System.Drawing.Point(12, 321)
        Me.btnRemoveItem.Name = "btnRemoveItem"
        Me.btnRemoveItem.Size = New System.Drawing.Size(95, 23)
        Me.btnRemoveItem.TabIndex = 7
        Me.btnRemoveItem.Text = "Remove Item"
        Me.btnRemoveItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnRemoveItem.UseVisualStyleBackColor = True
        '
        'btnDelete
        '
        Me.btnDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDelete.Image = Global.Delete_on_Reboot.My.Resources.Resources.delete
        Me.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnDelete.Location = New System.Drawing.Point(635, 32)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(145, 23)
        Me.btnDelete.TabIndex = 3
        Me.btnDelete.Text = "Stage Delete on Reboot"
        Me.btnDelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'btnBrowse
        '
        Me.btnBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnBrowse.Image = Global.Delete_on_Reboot.My.Resources.Resources.Browse
        Me.btnBrowse.Location = New System.Drawing.Point(749, 4)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(31, 23)
        Me.btnBrowse.TabIndex = 2
        Me.btnBrowse.UseVisualStyleBackColor = True
        '
        'listOperations
        '
        Me.listOperations.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) Or System.Windows.Forms.AnchorStyles.Left) Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.listOperations.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colFileName, Me.colRenamedTo})
        Me.listOperations.FullRowSelect = True
        Me.listOperations.Location = New System.Drawing.Point(12, 90)
        Me.listOperations.Name = "listOperations"
        Me.listOperations.Size = New System.Drawing.Size(768, 225)
        Me.listOperations.TabIndex = 10
        Me.listOperations.UseCompatibleStateImageBehavior = False
        Me.listOperations.View = System.Windows.Forms.View.Details
        '
        'colFileName
        '
        Me.colFileName.Text = "File Name"
        Me.colFileName.Width = 285
        '
        'colRenamedTo
        '
        Me.colRenamedTo.Text = "Renamed To"
        Me.colRenamedTo.Width = 221
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(792, 350)
        Me.Controls.Add(Me.listOperations)
        Me.Controls.Add(Me.btnStageRename)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.btnRemoveItem)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.chkAddToContextMenu)
        Me.Controls.Add(Me.btnDelete)
        Me.Controls.Add(Me.btnBrowse)
        Me.Controls.Add(Me.txtFile)
        Me.Controls.Add(Me.Label1)
        Me.MinimumSize = New System.Drawing.Size(808, 389)
        Me.Name = "Form1"
        Me.Text = "Delete on Reboot"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtFile As System.Windows.Forms.TextBox
    Friend WithEvents btnBrowse As System.Windows.Forms.Button
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents chkAddToContextMenu As System.Windows.Forms.CheckBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btnRemoveItem As System.Windows.Forms.Button
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnStageRename As System.Windows.Forms.Button
    Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
    Friend WithEvents listOperations As ListView
    Friend WithEvents colFileName As ColumnHeader
    Friend WithEvents colRenamedTo As ColumnHeader
End Class
