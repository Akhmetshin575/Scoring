   M S B A M L   `   `   `  ����C  ?Scoring3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:Scoring3Scoring3  5   	       Scoring3.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:Scoring3     5   	     � Title$9  3ООО Русфинанс Банк - Скоринг��5   	   $	��540��6.   $	��960��6;    � WindowState$ 	Maximized=�6G   .��5	      �  � ShowGridLines .   � RowDefinitions 5
   
   �� 5      $	��150 �6   �� 5      $��* �6   �� 5      $��* �6   .+�5   
   �� -   5         ��Name$ image��5      $��Logo.png��5      $��Center=�5      $	��100��5      $��Center=�5      $	��300��5      $��0��5      �� 5   
   -   5         ��Name$ textBox_login��5      $��Center=�5      $��Center=�5      $��	0 0 0 100q�5      $��1��5      $��25��5       �� TextWrapping$
 Wrap=�5      $��
Логин��5       $	��200��5!      $��14(�5"       �� TextAlignment$ Center=�5#      
�������
���   �5'      �� 5)   
   -   5        $ textBox_password��5*      $��Center=�5+      $��Center=�5,      $��1��5-      $��25��5.      $
 Wrap=�5/      $��Пароль��50      $	��200��51      $��14(�52      $ Center=�53      
�������
���   �56      �� 58   
   -   5         ��Name$ button��59      $��Вход��5:      $��Center=�5;      $��Center=�5<      $��	0 100 0 0q�5=      $��1��5>      $	��150��5?      $��30��5@      
���   �$
��Bold"�5B      	 �� 	IsDefault	 . 5D      G� 5F   
   -   5        
 G�Name$
 passwordBox��5G      $y�Hidden=�5H      $��1��5I      $��Center=�5J      $��Center=�5K      $	��200��5L      $��25��5M      $��14(�5N      5O      �� 5R   
   -   5         ��Name$ label_warning��5R      $�� ��5S      $��Center=�5T      $	��Top=�5U      $��2��5V      $��18(�5W      $
��Bold"�5X      
���  ��5Y       � 5[   
   -   5          �Name$ 	MainFrame��5[        � NavigationUIVisibility$ Hidden=�5\      $��Stretch=�5]      $��Stretch=�5^      $��3��5_      5`      5a      5b      