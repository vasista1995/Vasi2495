where t.updated between :StartTime and :EndTime
and t.lds_id = :LdsId
order by t.parameter_name, t.ch_id,ckc_id, t.sample_id
