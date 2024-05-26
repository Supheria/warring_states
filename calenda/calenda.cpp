// Clendar EXP++.cpp : 定义控制台应用程序的入口点。
//

#include <iostream>
#include <string>
#include <fstream>
#include <ctime>
#include <cstdlib>
using namespace std;

const int amount = 3000;
const int y_start = 1;
const int d_start = 1;
struct day_and_date
{
	int year[amount];
	int month[amount][12];
	int day[amount][12][31];
	int date[amount][12][31];
};

void cacu_days(day_and_date* exp);
enum YEAR { no_leap = 28, leap };
YEAR if_leap(int);
char sum_days(int, char);
int search(day_and_date* exp);
void cal_out(day_and_date* exp, int y);

int main()
{
	day_and_date* exp = new day_and_date;
	cacu_days(exp);
	int y = search(exp);

	cout << "Do you want to set the clendar of " << y;
	cout << " as a txt file(yes or no)?";
	char answer;
	cin >> answer;
	if (answer == 'n' || answer == 'N')
		cout << "Ok, thank you.";
	else if (answer == 'y' || answer == 'Y')
		cal_out(exp, y);
	else
		cout << "Emmm...";

	delete exp;
	clock_t start = clock();
	while (clock() - start <= 60 * CLOCKS_PER_SEC)
		;
}


void cacu_days(day_and_date* exp)
{
	int year = y_start;
	int DATE = d_start;

	for (int y = 0; y < amount; ++y)
	{
		exp->year[y] = year;
		for (int m = 0; m < 12; ++m)
		{
			exp->month[y][m] = m + 1;
			for (int d = 0; d < sum_days(year, exp->month[y][m]); ++d)
			{
				exp->day[y][m][d] = d + 1;
				if (year != y_start || m + 1 != 1 || d + 1 != 1)
				{
					if (DATE < 7)
						DATE++;
					else
						DATE = 1;
				}
				exp->date[y][m][d] = DATE;
			}
		}
		++year;
	}
}


YEAR if_leap(int year)
{
	if (year % 100 == 0)
		if ((year / 100) % 4 == 0)
			return leap;
		else
			return no_leap;
	else if (year % 100 != 0)
		if (year % 4 == 0)
			return leap;
		else
			return no_leap;
}


char sum_days(int year, char m)
{
	if (m == 1 || m == 3 || m == 5 || m == 7 || m == 8 || m == 10 || m == 12)
		return 31;
	else if (m == 2)
	{
		char mon2days = char(if_leap(year));
		return mon2days;
	}
	else
		return 30;
}


int search(day_and_date* exp)
{
	string weekdays[7] =
	{
		"Monday", "Tuesday", "Wednsday", "Thursday",
			"Friday", "Saturday", "Sunday"
	};
	cout << "Enter the year(" << y_start << " - " << y_start + amount - 1 << "): ";
	int y, m, d;//输入数字时，千万不能用char声明，因为cin会将其转换为输入字符的编码！
	cin >> y;
	int Y = y - y_start;
	cout << "Enter the month: ";
	cin >> m;
	cout << "Enter the day: ";
	cin >> d;
	if (y < y_start + amount && y >= y_start && m <= 12 && m>0 && d <= sum_days(y, m) && d > 0)
	{
		cout << "\nIt's " << weekdays[exp->date[Y][m - 1][d - 1] - 1] << "!\n";
		return y;
	}
	else
	{
		cout << "\nERRO! Bye~~~";
		delete exp;
		cin.get(); cin.get();
		exit(EXIT_FAILURE);
	}
	YEAR leap = if_leap(y);
	string str_leap[2] = { "Not Leap Year.", "Leap Year." };
	cout << y << " is " << str_leap[leap - 28] << endl << endl;
}

void cal_out(day_and_date* exp, int y)
{
	int Y = y - y_start;
	while (cin.get() != '\n')
		;
	cout << "Please name your calendar file(include .txt): ";
	string filename;
	getline(cin, filename);
	int s = filename.size();
	if ((s <= 4) || (filename[s - 1] != 't') || (filename[s - 2] != 'x')
		|| (filename[s - 3] != 't') || (filename[s - 4] != '.'))
		if (s = 0)
			filename += "OX.txt";
		else
			filename += ".txt";

	ofstream fout;
	fout.open(filename);
	if (!fout.is_open())
		cout << "\nCannot set new txt file!";
	else
	{
		fout << y << endl << endl;
		for (char m = 0; m < 12; ++m)
		{
			for (char d = 0; d < sum_days(y, exp->month[Y][m]); ++d)
			{
				if (exp->month[Y][m] < 10)
					fout << "0" << int(exp->month[Y][m]) << "月";
				else
					fout << int(exp->month[Y][m]) << "月";
				if (exp->day[Y][m][d] < 10)
					fout << "0" << int(exp->day[Y][m][d]) << "日\t";
				else
					fout << int(exp->day[Y][m][d]) << "日\t";
			}
			fout << endl << endl;
		}
		fout << "Have a good day ;)";
		cout << filename << " has successfully set!";
	}
}