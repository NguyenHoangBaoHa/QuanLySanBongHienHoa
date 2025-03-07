Create Procedure GetAllPitches
AS
Begin
	Select 
		p.Id,
		p.Name,
		pt.Name as PitchTypeName,
		pt.Price,
		pt.LimitPerson,
		IsNull((Select top 1 ImagePath from PitchTypeImage where PitchTypeId = pt.Id), 'default_image.png') as ImagePath,
		p.CreateAt,
		p.UpdateAt
	from Pitch p
	left join PitchType pt on p.IdPitchType = pt.Id
End;