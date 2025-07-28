where t.updated between :StartTime and :EndTime
and t.lds_id = :LdsId
and t.parameter_name not in ('GOF', 'Luftdruck')
and t.ext_sample_size < 1800
