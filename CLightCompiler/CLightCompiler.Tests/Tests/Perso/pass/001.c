int fac(int x) {
	if (x == 1)
		return 1;
	return fac(x - 1) * x;
}

int main() {
	out fac(6);
}