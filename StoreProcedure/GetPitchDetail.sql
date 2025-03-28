USE [QuanLySanBongHienHoa]
GO
/****** Object:  StoredProcedure [dbo].[GetPitchDetail]    Script Date: 11/03/2025 12:22:01 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetPitchDetail]
    @PitchId INT
AS
BEGIN
    SELECT
        p.Id,
        p.Name,
        pt.Id AS IdPitchType,
        pt.Name AS PitchTypeName,
        STRING_AGG(ptI.ImagePath, ', ') AS ListImagePath,
        pt.Price,
        pt.LimitPerson,
        p.CreateAt,
        p.UpdateAt
    FROM Pitch p
    LEFT JOIN PitchType pt ON pt.Id = p.IdPitchType
    LEFT JOIN PitchTypeImage ptI ON ptI.PitchTypeId = pt.Id
    WHERE p.Id = @PitchId -- Chỉ lấy dữ liệu cho 1 sân
    GROUP BY p.Id, p.Name, pt.Id, pt.Name, pt.Price, pt.LimitPerson, p.CreateAt, p.UpdateAt;
END;

