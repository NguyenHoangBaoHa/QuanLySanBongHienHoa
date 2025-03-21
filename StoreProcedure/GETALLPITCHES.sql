USE [QuanLySanBongHienHoa]
GO
/****** Object:  StoredProcedure [dbo].[GetAllPitches]    Script Date: 10/03/2025 12:27:39 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER Procedure [dbo].[GetAllPitches]
AS
Begin
	Select 
		p.Id,
		p.Name,
		pt.Id as IdPitchType,
		pt.Name as PitchTypeName,
		pt.Price,
		pt.LimitPerson,
		IsNull((Select top 1 ImagePath from PitchTypeImage where PitchTypeId = pt.Id), 'default_image.png') as ImagePath,
		p.CreateAt,
		p.UpdateAt
	from Pitch p
	left join PitchType pt on p.IdPitchType = pt.Id
End;