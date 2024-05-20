SELECT 
	p.id AS post_number, 
    count(pl.post_id) AS likes_per_post,
    p.user_id AS post_by_user
FROM 
	Posts p
LEFT JOIN 
	Post_likes pL ON p.id = pl.post_id
WHERE 
	p.user_id = 1 --insert the user-id of your choice here.
GROUP BY 
	p.id


