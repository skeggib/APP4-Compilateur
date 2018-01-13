int mult_mat_vec(int mat, int n, int m, int vec) {
	int i;
	int j;
	
	int res;
	res = alloc(n);

	for (i = 0; i < n; i = i + 1) {
		res[i] = 0;
		for (j = 0; j < n; j = j + 1) {
			res[i] = res[i] + mat[i * m + j] * vec[j];
		}
	}

	return res; 
}

int main() {
	int mat;
	mat = alloc(9);
	mat[0] = 1;
	mat[1] = 2;
	mat[2] = 3;
	mat[3] = 4;
	mat[4] = 5;
	mat[5] = 6;
	mat[6] = 7;
	mat[7] = 8;
	mat[8] = 9;

	int vec;
	vec = alloc(3);
	vec[0] = 1;
	vec[1] = 3;
	vec[2] = 2;

	int res;
	res = mult_mat_vec(mat, 3, 3, vec);

	int i;
	for (i = 0; i < 3; i = i + 1)
		out res[i];
}