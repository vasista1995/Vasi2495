where t.updated between :StartTime and :EndTime
and t.lds_id = :LdsId
and t.parameter_name not in ('GOF', 'Luftdruck')
and t.ext_sample_size < 1800
order by t.parameter_name, t.ch_id,ckc_id, t.sample_id
