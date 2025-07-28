where t.updated between :StartTime and :EndTime
and t.lds_id = :LdsId
and t.Exval_02 not in ('88888888','11111111','22222222','33333333','44444444','55555555',
'66666666','77777777','99999999','00112233','11223344','12345678','81628999')
and t.Exval_01 not in ('PELLET','DIFFUSION')
AND coalesce(TRIM(ckc_id),'0')='0'  
AND t.cf_value_04 ='HPS'
