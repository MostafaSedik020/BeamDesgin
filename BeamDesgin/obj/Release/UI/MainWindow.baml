   M S B A M L   `   `   `  ���� E  ABeamDesgin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:BeamDesgin.UIBeamDesgin.UI      BeamDesgin.UI.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     -localclr-namespace:BeamDesgin.UI     -   $	��800��  � Title$  Beam Desgin��
������  � WindowStyle$
 None=� � AllowsTransparency .  � 
ResizeMode$ CanResizeWithGrip=� � 	Resources  �� 
TargetType �� Property �� Value �� 
StartPoint	 �� EndPoint
 � Color � Offset �� Triggers P� Property P� Value-   'BeamDesgin.UI.RebarSizeToColorConverter   RebarSizeToColorConverter  RebarSize1CellStyle*  $System.Windows.Controls.DataGridCell �� 	Converter  RebarSize2CellStyle  RebarSize3CellStyle  RebarSize4CellStyle  RebarSize5CellStyle  RebarSize6CellStyle��%I  '��       '�� �    &	  X    &	 ]    7   &	 �    7   &	 �    7   &	     7   &	 Y    7   &	 �    7   �� " ��.���  � ��$	 100����  � ��$ 30����  � �� �#e7f4ff��  � ��$ Center=���  � ��$ Center=���  � �� �� $	 0,0&�$		 1,1&�.��� $
 #4d75aa��$	 0.0Q�� $
 #3b5d8f��$	 1.0Q���  � ��
 ���� ��  � ��$ 0q���  � ��$
 Bold"� P�  � {� .  . ���  � �� �� $	 0,0&�$		 1,1&�.��� $
 #e9edf5��$	 0.0Q�� $
 #e9edf5��$	 1.0Q���  � �� �#6f93c1�� " ��.���  � ��
 �   ���  � ��$
 Bold"���  � ��$ 5q���  � ��$ Center=���  � ��$ Center=���  � �� �� $	 0,0&�$		 1,1&�.��� $
 White��$	 0.0Q�� $
 White��$	 1.0Q���  � ��
 �   ���  � ��$ 2q� P�  �@d�	IsChecked$
 TrueY�. ���  � �� �� $	 0,0&�$		 1,1&�.��� $
 #a2b8d7��$	 0.0Q�� $
 #a2b8d7��$	 1.0Q���  � �� �#3b5d8f��  � ��
 �����P�  � {� .  . ���  � �� �� $	 0,0&�$		 1,1&�.��� $
 White��$	 0.0Q�� $
 White��$	 1.0Q���  � ��
 �������  � ��
 �����  �� "  .���  � �� �� *SelectedRebarSize1+8   �� "  .���  � �� �� *SelectedRebarSize2+8   �� "  .���  � �� �� *SelectedRebarSize3+8   �� "  .���  � �� �� *SelectedRebarSize4+8   �� "  .���  � �� �� *SelectedRebarSize5+8   �� "  .���  � �� �� *SelectedRebarSize6+8   .����  �� CornerRadius$ 25��$��1q�
������ 
������ ._�� ���� $	 0,0&�$		 1,1&�.��� $
 #e9edf5��$	 0.0Q�� $
 #cdd9ea��$	 0.5Q�� $
 #e9edf5��$	 1.0Q� � RowDefinitions �� $��30 ��� $��70 ��� $	��100 ��� $	��75* ��� $
��473* ��� $��70 �.+�� $��0��
������  � ColumnDefinitions �� $��* ��� $
��Auto �.+��� $��ProMark��$��Center=�$��10,0,0,0q�$��16(�$
��Bold"�$��Arial)�
���   �$��0���� -   $��X��$��30��$��30��
������ 
������ 
���   �$��1��� $��1�� �� $	��450 ��� $	��200 ��� $��* �.+��� -    ��Name$ Path_TxtBox��$
��Left=�$��20��$��20,0,0,0q�$��0�� �� TextWrapping$
 Wrap=�$��Center=�$	��400���� -   $��
Browse_btn��$��Browse��$��Center=�$��20,0,0,0q�$��1��$��Center=�$��70��$��30��� $��2�� �� $	��450 ��� $	��200 ��� $��* �.+�9� -    9�Name$ RebarExpander��$
��Left=�$��90��$��Choose Rebar��$��	24,10,0,0q�$��0��$	��Top=�$	��300��.��� ���#a2b8d7 �� $��* ��� $��* ��� $��* �.+��� $��0��.+��� $��8��$��0��$��0,2,6,2q��� $��10��$��1��$��0,2,0,2q��� $��12��$��2��$��0,2,0,2q��� $��1��.+��� $��14��$��0��$��0,2,0,2q��� $��16��$��1��$��0,2,0,2q��� $��18��$��2��$��0,2,0,2q��� $��2��.+��� $��20��$��0��$��0,2,0,2q��� $��22��$��1��$��0,2,0,2q��� $��25��$��2��$��0,2,0,2q���  �� Orientation$ Vertical=�$��Center=�$��Center=�$��1��.+��� -    ��Name$ 
desgin_btn��$��Design��$��	20,0,0,10q�$��1��$��70��$��30���� -   $ 	Clear_btn��$��Clear��$��70��$��20,0,0,0q�$��1���� $ Vertical=�$��Stretch=�$��Center=�$��2��.+��� -   $ missingBeam_TxtBox��$��45��$��0,0,10,5q�$
 Wrap=� �� 
IsReadOnly . �� -	   $ wrongBeam_TxtBox��$��45��$��0,5,10,0q�$
 Wrap=� . � $��5�� �� $��* ��� $��* �.+��� -
   $ 
Import_btn��$��Import to Revit��$	��170��$��Right=�$��	0,0,20,20q�$��1��$��Bottom=��� -   $ rebar_only_btn��$��Update Rebar only��$
��Left=�{�.  $	��170��$��Bottom=�$��0��$��200,15,0,20q��� -   $ 
Update_btn��$��Merge & Update��$	��170��$
��Left=�$��
20,15,0,20q�{�.  $��0��$��Bottom=�&   System.Windows.Controls.DataGrid  -     Name$ BeamDataGrid��$��3��   AutoGenerateColumns .     
IsReadOnly .  $��10,10,10,10q�   CanUserResizeRows .     CanUserResizeColumns .     CanUserSortColumns .     CanUserReorderColumns .      CanUserAddRows  .  $��2������ *
UserList+ =  7System.Windows.Controls.Primitives.DataGridColumnHeader��%�   '        '  �     �� "  .���  � �� �#acbfe6��  � ��$ SemiBold"���  � ��$ 0,0,1,2q���  � �� �#3b5d8f��  � ��$ Center=���  � ��$ Center=��� "  .���  �@��TextAlignment$ Center=�!   Columns! 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn"   Header$" Select��#   Width5  /System.Windows.Controls.DataGridLengthConverter$# 50 $   CellTemplate$ �� .R��� -   % d� 	IsChecked% �� *
IsSelected+& �� Mode$& TwoWay=�' �� UpdateSourceTrigger$' PropertyChanged=�0  *System.Windows.Controls.DataGridTextColumn  $" Count��$# 50 (   
IsReadOnly( . 1	  +System.Windows.Controls.DataGridBoundColumn) 	  Binding) �� *Count+$& OneWay=�  $" 	Beam Mark��$# * ( . ) �� *
BeamMark+$& OneWay=�  $" Breadth��$# * ( . ) �� *	Breadth+$& OneWay=�  $" Depth��$# * ( . ) �� *Depth+$& OneWay=�  $" Bot Left��$# * ( . ) �� *BOTTOM_RFT_CORNER+$& OneWay=�  $" Bot Mid��$# * ( . ) �� *BOTTOM_RFT_MID+$& OneWay=�  $" 	Bot Right��$# * ( . ) �� *BOTTOM_RFT_CORNER+$& OneWay=�4
  .System.Windows.Controls.DataGridComboBoxColumn
  $" Mid Dia.��$# 70 * 
  SelectedItemBinding* �� *SelectedRebarSize1+$& TwoWay=�$' PropertyChanged=�+ 
  ElementStyle+ �� " ��.���  � �� �� *
RebarSizes+��  � ��$ Center=���  � ��$ Center=� �� , �� Binding, �� *	IsEditing+- �� RelativeSource- �� . �� AncestorType$. DataGridRowL�/ �� Value/ True.a���  �@��IsDropDownOpen . 0 
  EditingElementStyle0 �� " ��.���  � �� �� *
RebarSizes+��  � ��$ Center=���  � ��$ Center=� �� , �� *	IsEditing+- �� $. DataGridRowL�/ True.a���  �@��IsDropDownOpen . 1   	CellStyle1 �� 2 �� ResourceKey$2 RebarSize1CellStyle��
  $" Corner Dia.��$# 70 * �� *SelectedRebarSize2+$& TwoWay=�$' PropertyChanged=�+ �� " ��.���  � �� �� *
RebarSizes+��  � ��$ Center=���  � ��$ Center=� �� , �� *	IsEditing+- �� $. DataGridRowL�/ True.a���  �@��IsDropDownOpen . 0 �� " ��.���  � �� �� *
RebarSizes+��  � ��$ Center=���  � ��$ Center=� �� , �� *	IsEditing+- �� $. DataGridRowL�/ True.a���  �@��IsDropDownOpen . 1 �� $2 RebarSize2CellStyle��  $" Top Left��$# * ( . ) �� *TOP_RFT_CORNER+$& OneWay=�  $" Top Mid��$# * ( . ) �� *TOP_RFT_MID+$& OneWay=�  $" 	Top Right��$# * ( . ) �� *TOP_RFT_CORNER+$& OneWay=�
  $" Mid Dia.��$# 70 * �� *SelectedRebarSize3+$& TwoWay=�$' PropertyChanged=�+ �� " ��.���  � �� �� *
RebarSizes+��  � ��$ Center=���  � ��$ Center=� �� , �� *	IsEditing+- �� $. DataGridRowL�/ True.a���  �@��IsDropDownOpen . 0 �� " ��.���  � �� �� *
RebarSizes+��  � ��$ Center=���  � ��$ Center=� �� , �� *	IsEditing+- �� $. DataGridRowL�/ True.a���  �@��IsDropDownOpen . 1 �� $2 RebarSize3CellStyle��
  $" Corner Dia.��$# 70 * �� *SelectedRebarSize4+$& TwoWay=�$' PropertyChanged=�+ �� " ��.���  � �� �� *
RebarSizes+��  � ��$ Center=���  � ��$ Center=� �� , �� *	IsEditing+- �� $. DataGridRowL�/ True.a���  �@��IsDropDownOpen . 0 �� " ��.���  � �� �� *
RebarSizes+��  � ��$ Center=���  � ��$ Center=� �� , �� *	IsEditing+- �� $. DataGridRowL�/ True.a���  �@��IsDropDownOpen . 1 �� $2 RebarSize4CellStyle��  $" 
Links Left��$# * ( . ) �� *LINKS_CORNER+$& OneWay=�  $" 	Links Mid��$# * ( . ) �� *	LINKS_MID+$& OneWay=�  $" Links right��$# * ( . ) �� *LINKS_CORNER+$& OneWay=�