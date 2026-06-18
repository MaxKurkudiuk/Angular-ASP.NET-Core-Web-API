export class AppUser {
  public fullName: string = '';
  public gender: string = '';

  get initials(): string {
    return this.fullName
      .split(' ')
      .map(n => n[0])
      .join('')
      .toUpperCase();
  }

  get genderIcon(): string {
    switch (this.gender?.toLowerCase()) {
      case 'male': return '👨';
      case 'female': return '👩';
      default: return '🧑';
    }
  }

}