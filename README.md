# StaffManagement
经典MIS，信息管理系统

使用前需要修改数据库连接字符串strConn，
从以下命令你可以了解该系统使用的数据库表设计：

CREATE TABLE [dbo].[tb_Staff](
	[No] [varchar](50) NULL,
	[Name] [varchar](50) NULL,
	[Salary] [float] NULL,
	[Evaluation] [varchar](50) NULL
) ON [PRIMARY]

