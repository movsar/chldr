# Set rate for all verified entries and translations

UPDATE xj_chldr.entry AS e
	JOIN xj_chldr.source AS s    
		ON e.source_id = s.source_id
    LEFT JOIN xj_chldr.translation AS t
		ON t.entry_id = e.entry_id
    SET e.Rate = 1000, t.Rate = 1000
    WHERE s.source_id IN ('63a816205d1af0e432fba6df','63a816205d1af0e432fba6e0','63a816205d1af0e432fba6e1','63ab2653d92da751cb251cbd','63ae6e7efd3e7896083feebe')    
;